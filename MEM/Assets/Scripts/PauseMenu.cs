using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    PlayerInputParent playerInputParent;
    GameObject mainCamera;
    Vector3 playerLastSavePos;
    Renderer rend;
    GameManager gameManager;

    public GameObject charactersClone;
    public GameObject pauseMenuCamera;

    public List<Animator> buttons = new List<Animator>();

    private void Awake()
    {
        /*rend = GetComponent<Renderer>();
        Color color = Color.white;
        color.a = 0;
        rend.material.color = color;*/
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void CallPauseMenu(PlayerInputParent instance)
    {
        playerInputParent = instance;
        playerInputParent.gameObject.SetActive(false);

        /*playerInputParent = instance;
        playerLastSavePos = playerInputParent.transform.position;

        //Temporarily set save point to pause menu
        var pauseMenuPos = transform.position;
        GameObject.FindGameObjectWithTag("SavePointController").GetComponent<SavePointController>().SetSavePosition(pauseMenuPos, 10 * Vector3.up, 10 * Vector3.down);
        //Spawn a clone of characters to pause menu
        playerInputParent.gameObject.SetActive(false);
        GameObject cpClone = Instantiate(charactersClone, pauseMenuPos, Quaternion.identity);
        cpClone.name = "cpClone";

        //Set camera
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.SetActive(false);
        pauseMenuCamera.SetActive(true);*/
    }

    public void ResumeGame()
    {
        //GameObject.FindGameObjectWithTag("SavePointController").GetComponent<SavePointController>().RevertSavePosition();
        
        //Reset Camera
        /*mainCamera.SetActive(true);
        pauseMenuCamera.SetActive(false);

        Destroy(GameObject.Find("cpClone"));*/
        playerInputParent.gameObject.SetActive(true);
        gameManager.rotationSpeedModifier = 1;
        gameManager.ChangeGameStateTo(GameManager.GameStates.InGame);
        EventSystem.current.SetSelectedGameObject(null);
        buttons[0].SetTrigger("Normal");
        foreach (Animator animator in buttons)
        {
            animator.SetTrigger("Normal");
        }
        
        this.gameObject.SetActive(false);
    }

    public void RestartLevel()
    {
        GameObject dontDestroyOnLoad = GameObject.Find("DontDestroyOnLoad");
        Destroy(dontDestroyOnLoad);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        GameObject dontDestroyOnLoad = GameObject.Find("DontDestroyOnLoad");
        Destroy(dontDestroyOnLoad);
        SceneManager.LoadScene("PlayableMenu");
    }
}
