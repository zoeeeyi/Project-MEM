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

    public Color changeToColor = Color.green;
    SavePointParent parent;
    AudioManager audioManager;
    Renderer rend;
    Color originalColor;
    Vector3 center;
    Vector3 originalScale;
    float floatingSmoothX;
    float floatingSmoothY;
    float scaleSmoothX;
    float scaleSmoothY;

    bool used = false;
    [HideInInspector] public bool sisterPointActivated = false;
    public SavePoint sisterPoint;
    float timer;

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        center = transform.position;
        originalScale = transform.localScale;
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
            Debug.Log("Parent used");
            Color newColor = changeToColor;
            newColor.a = .99f;
            rend.material.color = changeToColor;
            used = true;
        }

        //Visuals
        //Floating effect
        float newPosX = Mathf.SmoothDamp(transform.position.x, center.x + Random.Range(-parent.floatingRange, parent.floatingRange), ref floatingSmoothX, parent.floatingSmoothTime);
        float newPosY = Mathf.SmoothDamp(transform.position.y, center.y + Random.Range(-parent.floatingRange, parent.floatingRange), ref floatingSmoothY, parent.floatingSmoothTime);
        transform.position = new Vector3(newPosX, newPosY, transform.position.z);
        
        float newScaleX = Mathf.SmoothDamp(transform.localScale.x, originalScale.x + Random.Range(-parent.floatingScaleModifier, parent.floatingScaleModifier), ref scaleSmoothX, parent.floatingSmoothTime);
        float newScaleY = Mathf.SmoothDamp(transform.localScale.y, originalScale.y + Random.Range(-parent.floatingScaleModifier, parent.floatingScaleModifier), ref scaleSmoothY, parent.floatingSmoothTime);
        transform.localScale = new Vector3(newScaleX, newScaleY, transform.localScale.z);

        //Flash if the other savepoint under the same parent is activated
        if (sisterPointActivated)
        {
            if(timer < 2 * Mathf.PI)
            {
                timer += 0.01f;
            }
            else
            {
                timer = 0;
            }
            Color newColor = rend.material.color;
            newColor.a = Mathf.Abs(Mathf.Sin(timer));
            rend.material.color = newColor;
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
            audioManager.playAudioClip("CheckPoint");
            sisterPoint.sisterPointActivated = true;
            timer = 0;
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
            sisterPoint.sisterPointActivated = false;
        }
    }

}
