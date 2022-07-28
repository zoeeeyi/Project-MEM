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
    public int endPointNum = 2;
    public int endPointReached = 0;

    public PauseMenu pauseMenu;


    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        gameOverUI = GameObject.Find("GameOver");
        gameWinUI = GameObject.Find("GameWin");
        gameOverUI.SetActive(false);
        gameWinUI.SetActive(false);

        playerInputParent = GameObject.Find("Character Parent").GetComponent<PlayerInputParent>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.CallPauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (gameOver)
        {
            Time.timeScale = 0;
            gameOverUI.SetActive(true);
        }

        if (endPointNum == endPointReached)
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
