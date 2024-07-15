using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Object", order = 1)]
public class AudioObject : ScriptableObject
{
    [Header("Basic Information")]
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 3f)]
    public float pitch = 1f;
    public string key = "";

    [Header("Pitch Randomization")]
    public bool randomizePitch = false;
    [Range(0f, 3f)]
    public float minPitch = 0f;
    [Range(0f,3f)]
    public float maxPitch = 1f;

    [Header("Music Looping")]
    public float introLength;
    
    public void RandomizePitch()
    {
        if (!randomizePitch)
            return;

        pitch = Random.Range(minPitch, maxPitch);
    }
}
