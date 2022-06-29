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

    //Type 1: Move Block
    public GameObject moveBlock;
    public bool blockDisappear;
    MoveBlockController moveBlockController;

    void Start()
    {
        leftOpenValue = (leftOpen) ? 1 : -1;

        m_collider = GetComponent<Collider2D>();
        rend = GetComponent<Renderer>();
        m_color = rend.material.color;

        //Type 1: Move Block
        moveBlockController = moveBlock.GetComponent<MoveBlockController>();
        //moveBlock.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_collider.isTrigger) return;
        if (activated)
        {
            if (blockDisappear)
            {
                moveBlock.SetActive(false);
            }
            m_color.a = 0.5f;
            rend.material.color = m_color;
            moveBlockController.activated = true;
        }
        else
        {
            m_color.a = 1;
            rend.material.color = m_color;
            moveBlockController.activated = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.tag == "Player"))
        {
            m_color.a = 0.5f;
            rend.material.color = m_color;
            moveBlockController.activated = true;
        }
    }
}
