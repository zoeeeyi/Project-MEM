using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DisappearPlatform : RaycastController
{
    public LayerMask characterMask;
    float rayLength;

    [Header("Platform Setting")]
    //Auto Disappear
    public bool autoDisappear = false;
    [Range(0, 10)]
    public float autoDisappearTime;

    //Disappear
    [Range(0, 10)]
    public float disappearTime;

    //Reappear
    public bool reappear = false;
    [Range(0, 10)]
    public float reappearTime;

    Collider2D m_collider;
    Renderer rend;
    Color color;

    bool isDisappearing = false;
    float timer = 0;

    public override void Start()
    {
        base.Start();
        rayLength = 2 * skinWidth;

        m_collider = GetComponent<Collider2D>();
        rend = GetComponent<Renderer>();
        color = rend.material.color;
        timer = disappearTime;
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
            timer = Mathf.Max(timer - Time.deltaTime, 0);
            color.a = timer / disappearTime;
            rend.material.color = color;
            return;
        }

        if (autoDisappear)
        {
            StartCoroutine(AutoDisappearCoroutine());
            return;
        }

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
        autoDisappear = false;
        yield return new WaitForSeconds(autoDisappearTime);
        isDisappearing = true;
        yield return new WaitForSeconds(disappearTime);
        m_collider.enabled = false;
        if (reappear) StartCoroutine(ReappearCoroutine());
    }

    IEnumerator DisappearCoroutine()
    {
        isDisappearing = true;
        yield return new WaitForSeconds(disappearTime);
        m_collider.enabled = false;
        if (reappear) StartCoroutine(ReappearCoroutine());
    }

    IEnumerator ReappearCoroutine()
    {
        yield return new WaitForSeconds(reappearTime);
        m_collider.enabled = true;
        isDisappearing = false;
        color.a = 1;
        rend.material.color = color;
        timer = disappearTime;
        autoDisappear = true;
    }
}
