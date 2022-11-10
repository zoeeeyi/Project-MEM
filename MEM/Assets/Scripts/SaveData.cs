using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    static SaveData instance;
    [HideInInspector] public bool tutorialFinished { get; private set; }
    [SerializeField] bool resetData = false; //Dev purpose

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }

        //First time in game / game started but tutorial not finished / Manually reset data
        if (!PlayerPrefs.HasKey("TutorialFinished") || PlayerPrefs.GetInt("TutorialFinished") == 0 || resetData)
        {
            PlayerPrefs.SetInt("TutorialFinished", 0);
            tutorialFinished = false;
        } else
        {
            tutorialFinished = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTutorialFinish()
    {
        Debug.Log("Tutorial Finished!");
        tutorialFinished = true;
        PlayerPrefs.SetInt("TutorialFinished", 1);
    }
}
