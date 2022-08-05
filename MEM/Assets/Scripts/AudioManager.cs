using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    List<AudioSource> audioSourceList = new List<AudioSource>();
    Dictionary<string, AudioClip> AudioClipsDic = new Dictionary<string, AudioClip>();
    
    public List<AudioClips> AudioClipsList;

    void Start()
    {
        for (int i = 0; i < AudioClipsList.Count; i++)
        {
            AudioClipsDic.Add(AudioClipsList[i].clipName, AudioClipsList[i].clip);
        }
    }

    void LateUpdate()
    {
        foreach (AudioSource i in audioSourceList)
        {
            if (!i.isPlaying) audioSourceList.Remove(i);
        }
    }

    public void playAudioClip(string name)
    {
        AudioSource new_audioSource = gameObject.AddComponent<AudioSource>();
        new_audioSource.clip = AudioClipsDic[name];
        audioSourceList.Add(new_audioSource);
        audioSourceList[audioSourceList.Count - 1].Play();
    }
}

[Serializable]
public class AudioClips
{
    public string clipName;
    public AudioClip clip;
}
