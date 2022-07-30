using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SwitchController))]
public class LoadLevelSelector : MonoBehaviour
{
    public GameObject levelSelector;

    SwitchController switchController;
    Tilemap levelSelectorTiles;
    Color m_color;
    float m_transparency;

    void Start()
    {
        levelSelectorTiles = levelSelector.GetComponent<Tilemap>();
        switchController = GetComponent<SwitchController>();

        m_color = levelSelectorTiles.color;
        m_transparency = 0;

        levelSelector.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (switchController.activated)
        {
            levelSelector.SetActive(true);

            if (m_transparency < 1) m_transparency += Time.deltaTime;
            else
            {
                m_transparency = 1;
            }
        }
        else
        {
            if (m_transparency > 0) m_transparency -= Time.deltaTime;
            else
            {
                m_transparency = 0;
                levelSelector.SetActive(false);
            }
        }

        m_color.a = m_transparency;
        levelSelectorTiles.color = m_color;
    }
}
