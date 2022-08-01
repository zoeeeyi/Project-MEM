using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class LoadingSwitchController : SwitchController
{
    [Header("Timer Setting")]
    public float timeToOpen = 3;
    float timer;

    private void Awake()
    {
        Collider2D m_collider = GetComponent<Collider2D>();
        m_collider.isTrigger = true;
    }

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        timer = timeToOpen;
        animator.SetFloat("Timer", timer);
    }

    protected override void Update()
    {
        if (timer <= 0 && !activated)
        {
            activated = true;
            timer = 0;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //Leave blank
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && timer > 0 && !activated)
        {
            timer -= Time.deltaTime;
            animator.SetFloat("Timer", timer);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !activated)
        {
            timer = timeToOpen;
            animator.SetFloat("Timer", timer);
        }
    }
}
