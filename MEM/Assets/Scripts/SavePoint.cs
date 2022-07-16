using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]

public class SavePoint : MonoBehaviour
{
    public bool flipped;
    float playerAngle = 0;
    Collider2D player;

    SavePointParent parent;
    Renderer rend;
    Color originalColor;

    bool used = false;

    void Start()
    {
        if (flipped) playerAngle = 180;
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        parent = GetComponentInParent<SavePointParent>();
    }

    private void Update()
    {
        if (used) return;

        if (parent.used)
        {
            Color newColor = Color.green;
            rend.material.color = newColor;
            used = true;
        }
    }

    public void Reset()
    {
        if (!used)
        {
            parent.characters.Remove(player);
            rend.material.color = originalColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") 
            && collision.transform.localEulerAngles.z == playerAngle 
            && !parent.used)
        {
            player = collision;
            parent.characters.Add(collision);
            Color newColor = Color.green;
            newColor.a = 0.3f;
            rend.material.color = newColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")
            && collision.transform.localEulerAngles.z == playerAngle
            && !parent.used)
        {
            parent.characters.Remove(collision);
            rend.material.color = originalColor;
        }
    }
}
