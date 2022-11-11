using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    SwitchController m_switchController;
    void Start()
    {
        m_switchController = GetComponentInChildren<SwitchController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_switchController.activated)
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }
}
