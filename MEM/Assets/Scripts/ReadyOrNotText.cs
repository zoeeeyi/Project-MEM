using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadyOrNotText : MonoBehaviour
{
    TextMeshPro readyOrNotText;
    LoadingSwitchController parent;
    LevelSelectorSwitchParent levelSelectorSwitchParent;

    void Start()
    {
        readyOrNotText = GetComponent<TextMeshPro>();
        parent = GetComponentInParent<LoadingSwitchController>();
        levelSelectorSwitchParent = GameObject.Find("LevelSelectorSwitchParent").GetComponent<LevelSelectorSwitchParent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelSelectorSwitchParent.selectedLevel.Equals(""))
        {
            if (parent.activated) readyOrNotText.text = "Ready!";
        }
        else readyOrNotText.text = "Not Ready!";
    }
}
