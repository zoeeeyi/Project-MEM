using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    /*public bool requireBothCharacters = false;
    public bool playersReached;*/
    public bool leftOpen = true;
    [HideInInspector] public int leftOpenValue = 1;
    [HideInInspector] public bool activated = false;
    Renderer rend;
    Collider2D m_collider;
    Color m_color;

    [Header("Camera Setting")]
    public bool needCameraFocus = false;
    //public Vector3 camTargetPos;

    //Type 1: Move Block
    public bool blockDisappear;

    void Start()
    {
        leftOpenValue = (leftOpen) ? 1 : -1;

        m_collider = GetComponent<Collider2D>();
        rend = GetComponent<Renderer>();
        m_color = rend.material.color;

        if (!m_collider.isTrigger) needCameraFocus = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_collider.isTrigger) return;
        if (activated)
        {
            m_color.a = 0.5f;
            rend.material.color = m_color;
        }
        else
        {
            m_color.a = 1;
            rend.material.color = m_color;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.tag == "Player"))
        {
            m_color.a = 0.5f;
            rend.material.color = m_color;
            activated = true;
        }
    }
}
