using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class LevelIndicator : MonoBehaviour
{
    LevelSelectorSwitch parent;
    SpriteRenderer rend;
    Sprite closeSprite;

    public Sprite openSprite;

    void Start()
    {
        parent = GetComponentInParent<LevelSelectorSwitch>();
        rend = GetComponent<SpriteRenderer>();
        closeSprite = rend.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (parent.activated) rend.sprite = openSprite;
        else rend.sprite = closeSprite;
    }
}
