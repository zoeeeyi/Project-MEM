using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBeforeTutorial : MonoBehaviour
{
    [SerializeField] bool m_dontShowBeforeTutorial = true;
    SaveData m_saveData;
    // Start is called before the first frame update
    void Start()
    {
        m_saveData = GameObject.Find("SaveData").GetComponent<SaveData>();
        if (!m_saveData.tutorialFinished && m_dontShowBeforeTutorial) Destroy(gameObject);
        else if(m_saveData.tutorialFinished && !m_dontShowBeforeTutorial) Destroy(gameObject);
    }
}
