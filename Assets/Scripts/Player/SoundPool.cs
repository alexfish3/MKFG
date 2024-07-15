using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Animations;

public class SoundPool : MonoBehaviour
{
    private AudioSource[] sourcePool;

    private void Start()
    {
        GameObject sourceGO;
        sourcePool = new AudioSource[SoundManager.Instance.PoolSize];
        GameObject audioSource = SoundManager.Instance.AudioSourcePrefab;
        for (int i = 0; i < sourcePool.Length; i++)
        {
            sourceGO = Instantiate(audioSource, transform);
            sourcePool[i] = sourceGO.GetComponent<AudioSource>();
            sourcePool[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Plays a sound.
    /// </summary>
    /// <param name="key">Key of the sound in the SFX dictionary</param>
    /// <param name="position">Position the sound should come from</param>
    public void PlaySound(string key, Vector3 position, float timeStamp = 0f)
    {
        AudioSource sourceGO = GetAvailableSource();
        sourceGO.transform.position = position;
        SoundManager.Instance.PlaySFX(key, sourceGO, timeStamp);
    }

    /// <summary>
    /// This method resets an audio source so it is able to be pulled from the sound pool again.
    /// </summary>
    /// <param name="source">AudioSource to be reset</param>
    private void ResetSource(AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.gameObject.SetActive(false);
        SoundManager.Instance.SwitchSource(ref source, "SFX");
        source.volume = 1;
        source.pitch = 1;
        source.loop = false;
    }

    /// <summary>
    /// Gets an availabe audio source from the pool. Creates a new one if none are available.
    /// </summary>
    /// <returns></returns>
    public AudioSource GetAvailableSource()
    {
        foreach (AudioSource source in sourcePool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                return source;
            }
        }
        // hopefully won't get here, if it happens frequently we can increase the pool size in SM
        return Instantiate(SoundManager.Instance.AudioSourcePrefab, transform).GetComponent<AudioSource>();
    }

    /// <summary>
    /// Stops all audio playing from the source pool.
    /// </summary>
    public void StopAudio()
    {
        foreach (AudioSource source in sourcePool)
        {
            source.Stop();
        }
    }

    /// <summary>
    /// This coroutine is used for killing non-looping SFXs. It also won't be called with dedicated methods.
    /// </summary>
    /// <param name="source">Source to be killed after playback</param>
    /// <returns></returns>
    private IEnumerator KillSource(AudioSource source)
    {
        while (source.isPlaying)
        {
            yield return null;
        }
        ResetSource(source);
    }
}
