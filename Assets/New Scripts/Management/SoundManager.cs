using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;
using Random = UnityEngine.Random;

/// <summary>
/// This class is a singleton that plays audio. It has an audio source for game music and methods that play respective audio clips on passed in sources.
/// </summary>
public class SoundManager : SingletonMonobehaviour<SoundManager> 
{
    // music
    [Header("Sound Manager")]
    [Tooltip("Reference to the source that will play all the music.")]
    [SerializeField] private AudioSource musicSource; // testing this for timesample in update implementation

    private bool swappingTunes = true;
    private int totalLength, introLength;

    // dictionary stuff
    private Dictionary<string, AudioObject> sfxDictionary = new Dictionary<string, AudioObject>();
    private Dictionary<string, AudioMixerSnapshot> snapshotDictionary = new Dictionary<string, AudioMixerSnapshot>();

    [Header("Mixing Information")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerSnapshot gameplaySnapshot;
    [SerializeField] private AudioMixerSnapshot pausedSnapshot;

    [Header("Pooling Information")]
    [Tooltip("How many audio sources are in each player's pool")]
    [SerializeField] private int playerPoolSize = 10;
    [Tooltip("Prefab of the audio source GO")]
    [SerializeField] private GameObject audioSourcePrefab;
    public GameObject AudioSourcePrefab { get { return audioSourcePrefab; } }
    public int PoolSize { get { return playerPoolSize; } }

    [Header("Audio Clips")]
    [SerializeField] private AudioObject[] clips;

    /*[Header("Music")]
    [SerializeField] private AudioObject mainGameLoop;
    [SerializeField] private AudioObject mainMenuBGM;
    [SerializeField] private AudioObject finalOrderBGM;
    [SerializeField] private AudioObject resultsBGM;

    [Header("SFX")]
    [SerializeField] private AudioObject engineActive;
    [SerializeField] private AudioObject engineIdle;
    [SerializeField] private AudioObject brake;
    [SerializeField] private AudioObject boostUsed;
    [SerializeField] private AudioObject boostCharged;
    [SerializeField] private AudioObject miniBoost;
    [SerializeField] private AudioObject phasing;
    [SerializeField] private AudioObject orderPickup;
    [SerializeField] private AudioObject orderDropoff;
    [SerializeField] private AudioObject finalDropoff;
    [SerializeField] private AudioObject orderTheft;
    [SerializeField] private AudioObject death;
    [SerializeField] private AudioObject clockTowerBells;
    [SerializeField] private AudioObject timeoutWhistle;

    [SerializeField] private AudioObject[] driftSparks;

    [Header("UI")]
    [SerializeField] private AudioObject enter;
    [SerializeField] private AudioObject back;
    [SerializeField] private AudioObject scroll;
    [SerializeField] private AudioObject pause;


    [Header("Emotes")]
    [Tooltip("[0]: Top, [1]: Right, [2]: Bottom, [3]: Left")]
    [SerializeField] private AudioObject[] emoteSFX;
    [Range(0f, 1f)][SerializeField] private float emotePitchMin;
    [Range(1f, 2f)][SerializeField] private float emotePitchMax;

    [Header("Looping Logic")]
    [Tooltip("Length of the intro for the menu theme.")]
    [SerializeField] private float menuThemeIntroLength = 1.775f;
    [Tooltip("Length of the intro for the main gameplay theme.")]
    [SerializeField] private float gameThemeIntroLength = 1f;
    [Tooltip("Length of the intro for the final order theme.")]
    [SerializeField] private float finalThemeIntroLength = 1f;

    [Header("Debug")]
    [Tooltip("Will randomize noises to simulate multiple players.")]
    [SerializeField] private bool simulatePlayers;
    [Tooltip("We might not have player select music.")]
    [SerializeField] private bool playPlayerSelect;*/

    private bool shouldPlayMain = true;

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

    /// <summary>
    /// Here's where you'll find all the AudioKeys
    /// </summary>
    private void Start()
    {
        foreach(AudioObject clip in clips)
        {
            if (sfxDictionary.ContainsKey(clip.key))
            {
                Debug.LogError($"{clip.key} already exists in dictionary. Detected for {clip.name}. Changing key now");
                clip.key += $" {clip.name}";
            }
            sfxDictionary.Add(clip.key, clip);
        }
    }

    private void Update()
    {
        return; // will fix when more sounds are in
        if (swappingTunes)
        {
            return;
        }

        if (musicSource.timeSamples >= totalLength || !musicSource.isPlaying)
        {
            musicSource.timeSamples = introLength;
            musicSource.Play();
        }
    }

    public void PlaySFX(string key, AudioSource source, float timeStamp)
    {
        // init basic info
        source.clip = sfxDictionary[key].clip;
        source.volume = sfxDictionary[key].volume;
        sfxDictionary[key].RandomizePitch();
        source.pitch = sfxDictionary[key].pitch;

        // doesn't do anything right now but eventually you'll be able to play a clip at a specific time stamp
        source.timeSamples = Mathf.RoundToInt(timeStamp * source.clip.frequency);
        
        source.gameObject.SetActive(true);
        source.Play();
    }

    /// <summary>
    /// Switches the audio snapshot based on the provided key over the course of provided seconds.
    /// </summary>
    /// <param name="key">Key of the snapshot you're switching to</param>
    /// <param name="time">Time you want it to take, default is 0.1f</param>
    public void ChangeSnapshot(string key, float time = 0.1f)
    {
        if (snapshotDictionary.ContainsKey(key)) // this won't get called as often as SFX so I'm less worried about performance impact
        {
            snapshotDictionary[key].TransitionTo(time);
        }
    }

    /// <summary>
    /// This method returns the SFX from the dictionary with specified key. Returns null if clip can't be found and debugs the error.
    /// </summary>
    /// <param name="key">Key of the clip in the dictionary.</param>
    /// <returns></returns>
    public AudioObject GetSFX(string key)
    {
        AudioObject outClip;
        if (sfxDictionary.TryGetValue(key, out outClip))
        {
            return outClip;
        }
        return null;
    }

    /// <summary>
    /// This method switches the output mixer of an audio source, or debugs an error if the mixer couldn't be found.
    /// </summary>
    /// <param name="source">Source being switched</param>
    /// <param name="channel">Name of the output channel</param>
    public void SwitchSource(ref AudioSource source, string channel)
    {
        AudioMixerGroup targetGroup = mainMixer.FindMatchingGroups(channel)[0];
        if (targetGroup != null)
        {
            source.outputAudioMixerGroup = targetGroup;
        }
    }

    /// <summary>
    /// Switches the sources to passed in clip
    /// </summary>
    private void SwitchMusic(AudioObject inMusic, float inIntro)
    {
        if (inMusic.clip == musicSource.clip)
            return;

        swappingTunes = true; // won't do anything in update

        // calculate new lengths
        introLength = Mathf.RoundToInt(inIntro * inMusic.clip.frequency) + 1;
        totalLength = Mathf.RoundToInt(inMusic.clip.length * inMusic.clip.frequency);

        // init clip and volume
        musicSource.clip = inMusic.clip;
        musicSource.volume = inMusic.volume;

        musicSource.timeSamples = 0;
        musicSource.Play();

        swappingTunes = false;
    }

    // for volume control

    public void SetSFX(float value)
    {
        mainMixer.SetFloat("SFX", value);
    }

    public void SetMusic(float value)
    {
        mainMixer.SetFloat("Music", value);
    }
}
