using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SwitchHold : SwitchController
{
    [Header("Timer setting")]
    public float timeToOpen = 3;
    float timer;
    Renderer rend;

    private void Awake()
    {
        Collider2D m_collider = GetComponent<Collider2D>();
        m_collider.isTrigger = true;
    }

    void Start()
    {
        rend = GetComponent<Renderer>();

        timer = timeToOpen;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0 && !activated)
        {
            activated = true;
            timer = 0;

            Color m_color = rend.material.color;
            m_color.a = 0.5f;
            rend.material.color = m_color;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && timer > 0 && !activated)
        {
            timer -= Time.deltaTime;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !activated)
        {
            timer = timeToOpen;
        }
    }
}
