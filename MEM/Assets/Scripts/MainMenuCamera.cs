using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public Camera mainMenuCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainMenuCamera.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        mainMenuCamera.enabled = true;
    }
}
