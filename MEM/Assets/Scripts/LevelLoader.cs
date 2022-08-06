using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    protected List<LoadingSwitchController> loadingSwitchControllers;
    LevelSelectorSwitchParent levelSelectorSwitchParent;


    string sceneName;

    void Start()
    {
        levelSelectorSwitchParent = GameObject.Find("LevelSelectorSwitchParent").GetComponent<LevelSelectorSwitchParent>();

        LoadingSwitchController[] loadingSwitchControllerArray = GetComponentsInChildren<LoadingSwitchController>();
        loadingSwitchControllers = loadingSwitchControllerArray.ToList();
    }

    protected virtual void FixedUpdate()
    {
        sceneName = levelSelectorSwitchParent.selectedLevel;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!sceneName.Equals(""))
        {
            bool allActivated = true;
            foreach (var i in loadingSwitchControllers)
            {
                if (!i.activated) allActivated = false;
                
            }

            if (allActivated)
            {
                //Destroy "dont destroy onload objects" before leaving the scene
                GameObject dontDestroyOnLoad = GameObject.Find("DontDestroyOnLoad");
                Destroy(dontDestroyOnLoad);

                SceneManager.LoadScene(sceneName);
            }
        } 
        else
        {
            foreach (var i in loadingSwitchControllers)
            {
                i.Reset();
            }
        }
    }

    protected void SetSceneName(string n)
    {
        sceneName = n;
    }
}
