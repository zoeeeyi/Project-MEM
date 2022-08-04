using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    PlayerInputParent playerInputParent;
    GameObject mainCamera;
    Vector3 playerLastSavePos;

    public GameObject charactersClone;
    public GameObject pauseMenuCamera;

    public void CallPauseMenu(PlayerInputParent instance)
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.rotationSpeedModifier = 0;
        gameManager.inPauseMenu = true;


        playerInputParent = instance;
        playerLastSavePos = playerInputParent.transform.position;

        //Temporarily set save point to pause menu
        var pauseMenuPos = transform.position;
        GameObject.Find("SavePointController").GetComponent<SavePointController>().SetSavePosition(pauseMenuPos, 10 * Vector3.up, 10 * Vector3.down);
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
        GameObject.Find("SavePointController").GetComponent<SavePointController>().RevertSavePosition();
        
        //Reset Camera
        mainCamera.SetActive(true);
        pauseMenuCamera.SetActive(false);

        Destroy(GameObject.Find("cpClone"));
        playerInputParent.gameObject.SetActive(true);

        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.rotationSpeedModifier = 1;
        gameManager.inPauseMenu = false;
    }
}
