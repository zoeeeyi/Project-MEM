using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisPfChain : MonoBehaviour
{
    /*[Header("Chain Setting")]
    public bool firstOneAppear = true;*/

    [Header("Platform Setting")]
    //Auto Disappear
    bool autoDisappear = true;
    bool waitingToDisappear = false;
    [Range(0, 10)]
    public float autoDisappearWaitTime;

    //Disappear
    [Range(0, 10)]
    public float disappearTime;

    //Reappear
    bool reappear = true;
    float reappearTime;

    //Chain Setting
    [Range(0, 10)]
    public float bufferTime;

    [Header("Aesthetic Setting")]
    [Range(0, 1)]
    public float minTransparency = 0.1f;

    public List<GameObject> platformChainFirst = new List<GameObject>();
    public List<GameObject> platformChainSecond = new List<GameObject>();

    void Awake()
    {
        //float modifier = (firstOneAppear) ? 0 : 1;
        reappearTime = autoDisappearWaitTime + disappearTime;
        bufferTime = Mathf.Min(bufferTime, reappearTime);
        for (int i = 0; i < platformChainFirst.Count; i++)
        {
            DisappearPlatform disappearPlatformScript = platformChainFirst[i].GetComponent<DisappearPlatform>();
            disappearPlatformScript.autoDisappear = true;
            disappearPlatformScript.autoDisappearWaitTime = autoDisappearWaitTime;
            disappearPlatformScript.disappearTime = disappearTime;
            disappearPlatformScript.reappear = true;
            disappearPlatformScript.reappearTime = reappearTime;
            disappearPlatformScript.minTransparency = minTransparency;
            disappearPlatformScript.bufferTime = bufferTime;
            disappearPlatformScript.disappearOnStart = false;
            /*if ((i + modifier) % 2 == 1) disappearPlatformScript.disappearOnStart = true;
            else disappearPlatformScript.disappearOnStart = false;*/
        }

        for (int i = 0; i < platformChainSecond.Count; i++)
        {
            DisappearPlatform disappearPlatformScript = platformChainSecond[i].GetComponent<DisappearPlatform>();
            disappearPlatformScript.autoDisappear = true;
            disappearPlatformScript.autoDisappearWaitTime = autoDisappearWaitTime;
            disappearPlatformScript.disappearTime = disappearTime;
            disappearPlatformScript.reappear = true;
            disappearPlatformScript.reappearTime = reappearTime;
            disappearPlatformScript.minTransparency = minTransparency;
            disappearPlatformScript.bufferTime = bufferTime;
            disappearPlatformScript.disappearOnStart = true;
            /*if ((i + modifier) % 2 == 1) disappearPlatformScript.disappearOnStart = true;
            else disappearPlatformScript.disappearOnStart = false;*/
        }
    }
}