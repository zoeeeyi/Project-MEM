using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectedText : MonoBehaviour
{
    LevelSelectorSwitchParent parent;
    TextMeshPro selectedLevelText;

    // Start is called before the first frame update
    void Start()
    {
        parent = GetComponentInParent<LevelSelectorSwitchParent>();
        selectedLevelText = GetComponent<TextMeshPro>();
        selectedLevelText.text = "NONE";
    }

    // Update is called once per frame
    void Update()
    {
        selectedLevelText.text = parent.selectedLevel;
    }
}
