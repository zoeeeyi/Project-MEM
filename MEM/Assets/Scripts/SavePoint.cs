using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]

public class SavePoint : MonoBehaviour
{
    public bool flipped;
    float playerAngle = 0;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") 
            && collision.transform.localEulerAngles.z == playerAngle 
            && !parent.used)
        {
            parent.playerReached += 1;
            Color newColor = Color.green;
            newColor.a = 0.3f;
            rend.material.color = newColor;
            //if (!parent.characters.Contains(collision)) parent.characters.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")
            && collision.transform.localEulerAngles.z == playerAngle
            && !parent.used)
        {
            parent.playerReached -= 1;
            rend.material.color = originalColor;
            //if (parent.characters.Contains(collision)) parent.characters.Remove(collision);
        }
    }
}
