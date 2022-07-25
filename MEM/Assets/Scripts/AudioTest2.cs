using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest2 : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip Jump;
    public AudioClip Die;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            audioSource.PlayOneShot(Jump);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            audioSource.PlayOneShot(Die);
        }
    }
}
