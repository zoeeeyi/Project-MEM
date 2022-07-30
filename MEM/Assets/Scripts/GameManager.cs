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

    [Header("Test")]
    public GameObject test;
    public GameObject camera1;
    public GameObject camera2;
    Vector3 lastSavePos;

    [Header("Misc")]
    [HideInInspector] public float rotationSpeedModifier = 1; //use this when pausing & resuming game, value given by gameManager, can only be 0/1;

    private void Awake()
    {
        Time.timeScale = 1;
        rotationSpeedModifier = 1;
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
            lastSavePos = playerInputParent.transform.position;
            //var lastPos = playerInputParent.transform.position;
            var lastPos = new Vector3(-98.5999985f, -51.5f, -3);

            GameObject.Find("SavePointController").GetComponent<SavePointController>().lastSavePos = lastPos;

            playerInputParent.gameObject.SetActive(false);
            GameObject cpClone = Instantiate(test, lastPos, Quaternion.identity);
            cpClone.name = "cpClone";

            rotationSpeedModifier = 0;

            camera1.SetActive(false);
            camera2.SetActive(true);
            //pauseMenu.CallPauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject.Find("SavePointController").GetComponent<SavePointController>().lastSavePos = lastSavePos;

            camera1.SetActive(true);
            camera2.SetActive(false);

            Destroy(GameObject.Find("cpClone"));
            playerInputParent.gameObject.SetActive(true);
            rotationSpeedModifier = 1;
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
