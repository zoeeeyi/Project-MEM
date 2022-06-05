using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public bool reached = false;
    public bool Flipped;
    public CheckPoint checkPointFlipped;

    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (reached)
        {
            rend.material.color = Color.cyan;
        }

        if (!Flipped)
        {
            if (checkPointFlipped.reached)
            {
                Debug.Log("Checkpoint Reached!");
            }
        }
    }
}
