using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraChanger : MonoBehaviour
{
    MainMenuCameraController mainMenuCamera;

    public bool inLevelSeletor;

    void Start()
    {
        mainMenuCamera = GameObject.Find("MainMenu Camera").GetComponent<MainMenuCameraController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player") mainMenuCamera.inLevelSeletor = inLevelSeletor;
    }
}
