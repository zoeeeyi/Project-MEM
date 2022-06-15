using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public bool activated = false;
    private Renderer rend;

    public GameObject moveBlock;
    MoveBlockController moveBlockController;


    void Start()
    { 
        rend = GetComponent<Renderer>();
        moveBlockController = moveBlock.GetComponent<MoveBlockController>();
        moveBlock.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            rend.material.color = Color.cyan;
            moveBlock.SetActive(true);
        }
        else
        {
            moveBlock.SetActive(false);
        }
    }
}
