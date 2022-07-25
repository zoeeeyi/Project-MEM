using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioSource JumpSound;
    public List<audioClipDic> audioClipDic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            //JumpSound.clip = audioClipDic["Jump"];
            JumpSound.Play();
        }
    }
}

public class audioClipDic : AudioTest
{
    public AudioClip audioClip;
    public string audioName;

    /*protected AudioClip getAudioClip(string audioName)
    {

    }*/
}
