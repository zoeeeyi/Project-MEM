using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioSource JumpSound;
    public List<AudioClipSaver> audioClipSavers;
    Dictionary<string, AudioClip> audioDic;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < audioClipSavers.Count; i++)
        {
            string name = audioClipSavers[i].getAudioName();
            audioDic[name] = audioClipSavers[i].getAudioClip();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            JumpSound.clip = audioDic["Jump"];
            JumpSound.Play();
        }
    }
}