using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool gameOver = false;
    public GameObject gameOverUI;
    public GameObject gameWinUI;
    public PlayerInputParent playerInputParent;

    //Checkpoints
    public int checkPointNum = 2;
    public int checkPointReached = 0;

    private void Start()
    {
        playerInputParent = GameObject.Find("Character Parent").GetComponent<PlayerInputParent>();
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

        if (playerInputParent.state == PlayerInputParent.PlayerState.BeyondXGap) gameOver = true;
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
