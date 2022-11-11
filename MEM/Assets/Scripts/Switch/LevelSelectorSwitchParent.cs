using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSelectorSwitchParent : MonoBehaviour
{
    List<LevelSelectorSwitch> switchControllers;
    LevelSelectorSwitch lastActiveSwitch;

    [HideInInspector] public LevelSelectorSwitch currentlyActiveSwitch;
    [HideInInspector] public string selectedLevel = "";

    public GameObject arrow1;
    public GameObject arrow2;

    void Start()
    {
        var switchControllersArray = GetComponentsInChildren<LevelSelectorSwitch>();
        switchControllers = switchControllersArray.ToList();
        lastActiveSwitch = currentlyActiveSwitch;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Check if none of the levels is selected
        bool allSwitchOff = true;
        foreach (LevelSelectorSwitch i in switchControllers)
        {
            if (i.activated) allSwitchOff = false;
        }
        if (allSwitchOff)
        {
            selectedLevel = "";
            arrow1.SetActive(false);
            arrow2.SetActive(false);
        }

            if (lastActiveSwitch != currentlyActiveSwitch)
        {
            foreach (LevelSelectorSwitch i in switchControllers)
            {
                if (i != currentlyActiveSwitch)
                {
                    i.activated = false;

                    arrow1.SetActive(true);
                    arrow2.SetActive(true);
                }
            }

            lastActiveSwitch = currentlyActiveSwitch;
        }
    }
}
