
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Sprites;

[RequireComponent(typeof(Collider2D))]
public class LoadingSwitchController : SwitchController
{
    [Header("Timer Setting")]
    public float timeToOpen = 2;
    //float timer;

    [Header("Sprite Setting")]
    public List<Sprite> spriteList;
    SpriteRenderer spriteRenderer;
    int currentSpriteIndex;

    private void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Collider2D m_collider = GetComponent<Collider2D>();
        m_collider.isTrigger = true;
    }

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
        Reset();
    }

    protected override void Update()
    {
        if (spriteList.Count == 0)
        {
            Debug.Log("Loading Switch Need Sprites!");
            return;
        }
        /*if (timer <= 0 && !activated)
        {
            activated = true;
            timer = 0;
        }*/

        if (currentSpriteIndex == spriteList.Count)
        {
            activated = true;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated)
        {
            spriteRenderer.sprite = spriteList[1];
            audioManager.playAudioClip("SwitchLoad");
            currentSpriteIndex += 1;
            StartCoroutine(PlayNextSprite());
        }
    }

    IEnumerator PlayNextSprite()
    {
        yield return new WaitForSeconds(timeToOpen / 3);
        if (currentSpriteIndex < spriteList.Count)
        {
            spriteRenderer.sprite = spriteList[currentSpriteIndex];
            audioManager.playAudioClip("SwitchLoad");
            currentSpriteIndex++;
            if (currentSpriteIndex < spriteList.Count)
            {
                StartCoroutine(PlayNextSprite());
            }
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && timer > 0 && !activated)
        {
            timer -= Time.deltaTime;
            animator.SetFloat("Timer", timer);
        }
    }*/

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !activated)
        {
            Reset();
        }
    }

    public void Reset()
    {
        StopAllCoroutines();
        currentSpriteIndex = 0;
        spriteRenderer.sprite = spriteList[0];
        /*timer = timeToOpen;
        animator.SetFloat("Timer", timer);*/
        activated = false;
    }
}
