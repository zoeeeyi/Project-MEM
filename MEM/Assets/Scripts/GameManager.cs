using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool gameOver = false;
    public GameObject gameOverUI;
    public GameObject gameWinUI;

    //Checkpoints
    public int checkPointNum = 2;
    public int checkPointReached = 0;

    private void Start()
    {
        Time.timeScale = 1;
        gameOverUI = GameObject.Find("GameOver");
        gameWinUI = GameObject.Find("GameWin");
        gameOverUI.SetActive(false);
        gameWinUI.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (gameOver)
        {
            Time.timeScale = 0;
            gameOverUI.SetActive(true);
        }

        if (checkPointNum == checkPointReached)
        {
            Time.timeScale = 0;
            gameWinUI.SetActive(true);
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
