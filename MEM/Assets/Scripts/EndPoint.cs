using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public bool reached = false;
    public bool Flipped;
    public EndPoint endPointFlipped;

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
            this.gameObject.SetActive(false);
        }

        if (!Flipped)
        {
            if (endPointFlipped.reached)
            {
                Debug.Log("Checkpoint Reached!");
            }
        }
    }
}
