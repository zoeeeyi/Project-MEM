using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerDontDestroy : MonoBehaviour
{
    AudioManager audioManager;
    GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //Audio Setup
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.playAudioClip(gameManager.bgmName);
    }
}
