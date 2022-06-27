using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DisappearPlatform : RaycastController
{
    public LayerMask characterMask;
    float rayLength;

    [Header("Platform Setting")]
    [Range(0, 10)]
    public float disappearTime;

    Renderer rend;
    Color color;

    bool isDisappearing = false;
    float timer = 0;

    public override void Start()
    {
        base.Start();
        rayLength = 2 * skinWidth;

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
            timer -= Time.deltaTime;
            color.a = timer / disappearTime;
            rend.material.color = color;
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
    IEnumerator DisappearCoroutine()
    {
        isDisappearing = true;
        yield return new WaitForSeconds(disappearTime);
        gameObject.SetActive(false);
    }
}
