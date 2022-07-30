using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    //For pause menu
    GameObject[] allGameObjects;
    Vector3 playerLastPosition;

    // Start is called before the first frame update
    void Start()
    {
        /*allGameObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject i in pauseMenuDontDestroyList)
        {
            foreach (Transform child in i.transform)
            {
                pauseMenuDontDestroyList.Add(child.gameObject);
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallPauseMenu()
    {
        var player = GameObject.Find("Character");
        var playerFlipped = GameObject.Find("CharacterFlipped");

        playerFlipped.SetActive(false);
        playerLastPosition = player.transform.position;

        player.transform.position = transform.position;
        
        /*foreach (GameObject i in allGameObjects)
        {
            if(!pauseMenuDontDestroyList.Contains(i)) i.SetActive(false);
        }
        Instantiate(pauseMenu, player.transform.position, Quaternion.identity);
        player.SetActive(true);*/
    }
}
