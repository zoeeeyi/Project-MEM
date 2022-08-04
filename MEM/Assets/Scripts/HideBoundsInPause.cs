using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideBoundsInPause : MonoBehaviour
{
    void Awake()
    {
        Renderer rend = GetComponent<Renderer>();
        Color color = Color.white;
        color.a = 0;
        rend.material.color = color;
    }
}
