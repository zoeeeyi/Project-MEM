using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SwitchController))]
public class LoadLevelSelector : MonoBehaviour
{
    public GameObject levelSelectorOtherObjects;
    public GameObject levelSelectorTilemap;

    SwitchController switchController;
    Tilemap levelSelectorTiles;
    Color m_color;
    float m_transparency;

    void Start()
    {
        levelSelectorTiles = levelSelectorTilemap.GetComponent<Tilemap>();
        switchController = GetComponent<SwitchController>();

        m_color = levelSelectorTiles.color;
        m_transparency = 0;

        levelSelectorTilemap.SetActive(false);
        levelSelectorOtherObjects.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (switchController.activated)
        {
            levelSelectorTilemap.SetActive(true);

            if (m_transparency < 1) m_transparency += Time.deltaTime;
            else
            {
                m_transparency = 1;
                levelSelectorOtherObjects.SetActive(true);
            }
        }
        else
        {
            if (m_transparency > 0) m_transparency -= Time.deltaTime;
            else
            {
                m_transparency = 0;
                levelSelectorTilemap.SetActive(false);
                levelSelectorOtherObjects.SetActive(false);
            }
        }

        m_color.a = m_transparency;
        levelSelectorTiles.color = m_color;
    }
}
