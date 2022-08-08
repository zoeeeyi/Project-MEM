using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSwitchSound : MonoBehaviour
{
    AudioManager audioManager;
    public int playSound;

    void Start()
    {
        playSound = 0;
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void FixedUpdate()
    {
        if (playSound == 1)
        {
            audioManager.playAudioClip("SwitchLoad");
            playSound = 0;
        }
    }
}
