using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public bool activated = false;
    private Renderer rend;

    //Type 1: Move Block
    public GameObject moveBlock;
    MoveBlockController moveBlockController;

    void Start()
    { 
        rend = GetComponent<Renderer>();

        //Type 1: Move Block
        moveBlockController = moveBlock.GetComponent<MoveBlockController>();
        //moveBlock.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            rend.material.color = new Color(0, 255/255f, 0, 255/255f);
            moveBlockController.activated = true;
        }
        else
        {
            rend.material.color = new Color(255/255f, 0/255f, 0/255f, 255/255f);
            moveBlockController.activated = false;
        }
    }
}
