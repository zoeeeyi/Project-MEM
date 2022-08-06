using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LevelLoaderEndPoint : LevelLoader
{
    public string nextSceneName;
    private void Start()
    {
        SetSceneName(nextSceneName);
        LoadingSwitchController[] loadingSwitchControllerArray = GetComponentsInChildren<LoadingSwitchController>();
        loadingSwitchControllers = loadingSwitchControllerArray.ToList();
    }

    protected override void FixedUpdate()
    {
        //Leave blank
    }
}
