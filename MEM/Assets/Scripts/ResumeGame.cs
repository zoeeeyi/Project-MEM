using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeGame : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PauseMenu pauseMenu = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();
            pauseMenu.ResumeGame();
        }
    }
}
