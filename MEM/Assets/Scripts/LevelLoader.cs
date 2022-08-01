using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    List<LoadingSwitchController> loadingSwitchControllers;

    public string sceneName;

    void Start()
    {
        LoadingSwitchController[] loadingSwitchControllerArray = GetComponentsInChildren<LoadingSwitchController>();
        loadingSwitchControllers = loadingSwitchControllerArray.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        bool allActivated = true;
        foreach(var i in loadingSwitchControllers)
        {
            if(!i.activated) allActivated = false;
        }
        if (allActivated)
        {
            //Destroy "dont destroy onload objects" before leaving the scene
            GameObject dontDestroyOnLoad = GameObject.Find("DontDestroyOnLoad");
            Destroy(dontDestroyOnLoad);

            SceneManager.LoadScene(sceneName);
        }
    }
}
