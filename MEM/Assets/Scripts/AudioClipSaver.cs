using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipSaver : MonoBehaviour
{
    public AudioClip audioClip;
    public string audioName;

    public string getAudioName()
    {
        return audioName;
    }

    public AudioClip getAudioClip()
    {
        return audioClip;
    }
}
