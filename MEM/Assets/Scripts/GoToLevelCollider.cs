using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLevelCollider : MonoBehaviour
{
    public string goToLevelName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject dontDestroyOnLoad = GameObject.Find("DontDestroyOnLoad");
            Destroy(dontDestroyOnLoad);

            SceneManager.LoadScene(goToLevelName);
        }
    }
}
