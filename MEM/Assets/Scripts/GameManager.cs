using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class GameManager : MonoBehaviour
{
    //PauseMenu
    PauseMenu pauseMenu;
    [HideInInspector] public bool inPauseMenu;

    //UI
    Volume m_volume;
    VolumeProfile m_volProfile;
    float volumeWeightSmoothV;
    public float volumeWeightSmoothTime = 0.3f;
    public GameObject gameOverUI;
    public GameObject gameWinUI;
    public PlayerInputParent playerInputParent;
    public TextMeshProUGUI debugBox;

    //Checkpoints
    public int endPointNum = 2;
    public int endPointReached = 0;

    //Misc
    [Header("Misc")]
    [HideInInspector] public float rotationSpeedModifier = 1; //use this when pausing & resuming game, value given by gameManager, can only be 0/1;

    [Header("Audio Setting")]
    AudioManager audioManager;
    public string bgmName;

    //Game States
    GameStates gameState;
    public enum GameStates
    {
        InGame,
        GameOver,
        inPauseMenu
    }

    private void Awake()
    {
        gameState = GameStates.InGame;
        Time.timeScale = 1;
        rotationSpeedModifier = 1;
    }

    private void Start()
    {
        //Pause Menu Setup
        pauseMenu = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();
        pauseMenu.gameObject.SetActive(false);
        inPauseMenu = false;

        //UI Setup
        m_volume = GameObject.Find("Global Volume").GetComponent<Volume>();
        m_volProfile = m_volume.sharedProfile;
        m_volume.weight = 1;

        gameOverUI = GameObject.Find("GameOver");
        gameWinUI = GameObject.Find("GameWin");
        gameOverUI.SetActive(false);
        gameWinUI.SetActive(false);

        playerInputParent = GameObject.Find("Character Parent").GetComponent<PlayerInputParent>();
    }
    void Update()
    {
        if(debugBox != null) debugBox.text = System.Math.Round(1.0f / Time.deltaTime, 2).ToString();
        //Universal Controlls
        if (Input.GetButtonDown("Restart"))
        {
            if (gameState != GameStates.inPauseMenu)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            gameState = GameStates.GameOver;
        }


        //Game State Specific Controlls
        switch (gameState)
        {
            case GameStates.InGame:
                //UI
                if (m_volume.weight != 0)
                {
                    float _smoothTime = 1;
                    GameObject _levelTransition = GameObject.Find("Level Transition");
                    if (_levelTransition != null) _smoothTime = _levelTransition.GetComponent<LevelTransition>().levelTransitionTime;
                    m_volume.weight = Mathf.SmoothDamp(m_volume.weight, 0, ref volumeWeightSmoothV, _smoothTime);
                }

                //Inputs
                if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "PlayableMenu")
                {
                    pauseMenu.gameObject.SetActive(true);
                    //pauseMenu.CallPauseMenu(playerInputParent);
                    playerInputParent.freezeMovement = true;
                    rotationSpeedModifier = 0;
                    //m_volume.weight = 1;
                    gameState = GameStates.inPauseMenu;
                }
                break;

            case GameStates.GameOver:
                //UI
                if (m_volume.weight != 1)
                {
                    m_volume.weight = Mathf.SmoothDamp(m_volume.weight, 1, ref volumeWeightSmoothV, volumeWeightSmoothTime);
                }
                playerInputParent.gameObject.SetActive(false);
                gameOverUI.SetActive(true);
                break;

            case GameStates.inPauseMenu:
                //UI
                if (m_volume.weight != 1)
                {
                    m_volume.weight = Mathf.SmoothDamp(m_volume.weight, 1, ref volumeWeightSmoothV, volumeWeightSmoothTime);
                }

                //Inputs
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    pauseMenu.ResumeGame();
                    //pauseMenu.gameObject.SetActive(false);
                    //m_volume.weight = 1;
                    //rotationSpeedModifier = 1;
                    //gameState = GameStates.InGame;
                }
                break;
        }


        if (endPointNum == endPointReached)
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

    public void ChangeGameStateTo(GameStates s)
    {
        gameState = s;
    }

    public GameStates GetGameState()
    {
        return gameState;
    }
}
