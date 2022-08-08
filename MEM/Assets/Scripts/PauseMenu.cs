using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    PlayerInputParent playerInputParent;
    GameObject mainCamera;
    Vector3 playerLastSavePos;
    Renderer rend;

    public GameObject charactersClone;
    public GameObject pauseMenuCamera;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        Color color = Color.white;
        color.a = 0;
        rend.material.color = color;
    }

    public void CallPauseMenu(PlayerInputParent instance)
    {
        playerInputParent = instance;
        playerLastSavePos = playerInputParent.transform.position;

        //Temporarily set save point to pause menu
        var pauseMenuPos = transform.position;
        GameObject.FindGameObjectWithTag("SavePointController").GetComponent<SavePointController>().SetSavePosition(pauseMenuPos, 10 * Vector3.up, 10 * Vector3.down);
        //Spawn a clone of characters to pause menu
        playerInputParent.gameObject.SetActive(false);
        GameObject cpClone = Instantiate(charactersClone, pauseMenuPos, Quaternion.identity);
        cpClone.name = "cpClone";

        //Set camera
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.SetActive(false);
        pauseMenuCamera.SetActive(true);
    }

    public void ResumeGame()
    {
        GameObject.FindGameObjectWithTag("SavePointController").GetComponent<SavePointController>().RevertSavePosition();
        
        //Reset Camera
        mainCamera.SetActive(true);
        pauseMenuCamera.SetActive(false);

        Destroy(GameObject.Find("cpClone"));
        playerInputParent.gameObject.SetActive(true);
    }
}
