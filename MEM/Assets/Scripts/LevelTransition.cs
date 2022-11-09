using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public float levelTransitionTime;
    DontDestroyOnLoad m_ddol;
    TextMeshProUGUI m_tmp;
    Color m_color;
    float m_colorSmoothV;

    // Start is called before the first frame update
    void Start()
    {
        m_ddol = GameObject.Find("DontDestroyOnLoad").GetComponent<DontDestroyOnLoad>();
        m_tmp = GetComponent<TextMeshProUGUI>();

        if (m_ddol.levelStarted) Destroy(gameObject);
        else
        {
            m_tmp.text = SceneManager.GetActiveScene().name;
            m_ddol.levelStarted = true;
            m_color = m_tmp.color;
            StartCoroutine(LevelTransitionRoutine());
        }
    }

    private void Update()
    {
        m_color.a = Mathf.SmoothDamp(m_color.a, 0, ref m_colorSmoothV, levelTransitionTime);
        m_tmp.color = m_color;
    }

    IEnumerator LevelTransitionRoutine()
    {
        yield return new WaitForSeconds(levelTransitionTime);
        Destroy(gameObject);
    }
}
