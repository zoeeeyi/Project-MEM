using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    AudioSource temp;
    List<AudioSource> audioSourceList = new List<AudioSource>();
    Dictionary<string, AudioClips> AudioClipsDic = new Dictionary<string, AudioClips>();
    
    public List<AudioClips> AudioClipsList;

    void Start()
    {
        temp = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < AudioClipsList.Count; i++)
        {
            AudioClipsDic.Add(AudioClipsList[i].clipName, AudioClipsList[i]);
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < audioSourceList.Count; i++)
        {
            if (!audioSourceList[i].isPlaying)
            {
                AudioSource deleteThis = audioSourceList[i];
                audioSourceList[i] = temp;
                Destroy(deleteThis);
                audioSourceList.Remove(audioSourceList[i]);
            }
        }
    }

    public void playAudioClip(string name)
    {
        AudioSource new_audioSource = gameObject.AddComponent<AudioSource>();
        new_audioSource.clip = AudioClipsDic[name].clip;
        new_audioSource.volume = AudioClipsDic[name].volume;
        new_audioSource.loop = AudioClipsDic[name].loop;
        audioSourceList.Add(new_audioSource);
        audioSourceList[audioSourceList.Count - 1].Play();
    }
}

[Serializable]
public class AudioClips
{
    public string clipName;
    public AudioClip clip;
    [Range(0, 1)]
    public float volume;
    public bool loop;
}
