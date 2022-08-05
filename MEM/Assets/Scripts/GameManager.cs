using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //PauseMenu
    PauseMenu pauseMenu;
    [HideInInspector] public bool inPauseMenu;

    public bool gameOver = false;
    public GameObject gameOverUI;
    public GameObject gameWinUI;
    public PlayerInputParent playerInputParent;

    //Checkpoints
    public int endPointNum = 2;
    public int endPointReached = 0;

    [Header("Misc")]
    [HideInInspector] public float rotationSpeedModifier = 1; //use this when pausing & resuming game, value given by gameManager, can only be 0/1;

    [Header("Audio Setting")]
    AudioManager audioManager;
    public string bgmName;

    private void Awake()
    {
        Time.timeScale = 1;
        rotationSpeedModifier = 1;
    }

    private void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.playAudioClip(bgmName);
        pauseMenu = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();
        inPauseMenu = false;

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
            if (!inPauseMenu) pauseMenu.CallPauseMenu(playerInputParent);
            else pauseMenu.ResumeGame();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!inPauseMenu)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
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
