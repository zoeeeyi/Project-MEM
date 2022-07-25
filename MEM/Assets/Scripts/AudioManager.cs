using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClips> AudioClipsList;
    Dictionary<string, AudioClip> AudioClipsDic;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        foreach(AudioClips i in AudioClipsList)
        {
            AudioClipsDic[i.clipName] = i.clip;
        }
    }

    public AudioClip getAudioClip(string name)
    {
        AudioClip clip = AudioClipsDic[name];
        return clip;
    }
}

[Serializable]
public class AudioClips
{
    public string clipName;
    public AudioClip clip;
}
