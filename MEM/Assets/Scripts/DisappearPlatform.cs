using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DisappearPlatform : RaycastController
{
    public LayerMask characterMask;
    float rayLength;

    [Header("Platform Setting")]
    public bool disappearOnStart = false;
    //Auto Disappear
    public bool autoDisappear = false;
    bool waitingToDisappear = false;
    [Range(0, 10)]
    public float autoDisappearWaitTime;

    //Disappear
    [Range(0, 10)]
    public float disappearTime;

    //Reappear
    public bool reappear = false;
    [Range(0, 10)]
    public float reappearTime;

    //Chain Setting
    [HideInInspector] public float bufferTime = 0;

    [Header("Aesthetic Setting")]
    [Range(0, 1)]
    public float minTransparency = 0.1f;

    Collider2D m_collider;
    Renderer rend;
    Color color;
    AudioManager audioManager;

    bool isDisappearing = false;
    float timer = 0;

    public override void Start()
    {
        base.Start();
        rayLength = 2 * skinWidth;

        m_collider = GetComponent<Collider2D>();
        rend = GetComponent<Renderer>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        color = rend.material.color;
        timer = disappearTime;

        if (disappearOnStart)
        {
            m_collider.enabled = false;
            timer = 0;
            isDisappearing = true;
            StartCoroutine(ReappearCoroutine(bufferTime));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisappearing)
        {
            /*if (timer >= Mathf.PI)
            {
                timer = 0;
            } else timer += Time.deltaTime * 10;

            color.a = Mathf.Cos(timer);*/
            timer = Mathf.Max(timer - Time.deltaTime, minTransparency * disappearTime);
            color.a = timer / disappearTime;
            rend.material.color = color;
            return;
        }

        if (autoDisappear && !waitingToDisappear)
        {
            StartCoroutine(AutoDisappearCoroutine());
            return;
        }

        if (autoDisappear) return;
        UpdateRaycastOrigins();

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, characterMask);
            Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);

            if (hit && hit.distance != 0)
            {
                StartCoroutine(DisappearCoroutine());
                break;
            }

            rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, characterMask);
            Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);

            if (hit && hit.distance != 0)
            {
                StartCoroutine(DisappearCoroutine());
                break;
            }
        }
    }

    IEnumerator AutoDisappearCoroutine()
    {
        waitingToDisappear = true;
        yield return new WaitForSeconds(autoDisappearWaitTime);
        isDisappearing = true;
        yield return new WaitForSeconds(disappearTime);
        m_collider.enabled = false;
        if (reappear) StartCoroutine(ReappearCoroutine(2 * bufferTime));
    }

    IEnumerator DisappearCoroutine()
    {
        StartCoroutine(PlayDisappearSound());
        isDisappearing = true;
        yield return new WaitForSeconds(disappearTime);
        m_collider.enabled = false;
        if (reappear) StartCoroutine(ReappearCoroutine(2 * bufferTime));
    }

    IEnumerator ReappearCoroutine(float bufferTime)
    {
        yield return new WaitForSeconds(reappearTime - bufferTime);
        m_collider.enabled = true;
        isDisappearing = false;
        color.a = 1;
        rend.material.color = color;
        timer = disappearTime;
        waitingToDisappear = false;
    }

    IEnumerator PlayDisappearSound()
    {
        yield return new WaitForSeconds(0f * disappearTime);
        audioManager.playAudioClip("DisappearPlat");
    }
}
