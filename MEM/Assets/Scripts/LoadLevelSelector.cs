using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SwitchController))]
public class LoadLevelSelector : MonoBehaviour
{
    public GameObject levelSelector;

    SwitchController switchController;

    void Start()
    {
        switchController = GetComponent<SwitchController>();
        levelSelector.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (switchController.activated) levelSelector.SetActive(true);
        else levelSelector.SetActive(false);
    }
}
