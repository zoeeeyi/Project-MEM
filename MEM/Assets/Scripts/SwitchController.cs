using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    /*public bool requireBothCharacters = false;
    public bool playersReached;*/
    public bool leftOpen = true;
    [HideInInspector] public int leftOpenValue = 1;
    [HideInInspector] public bool activated = false;
    private Renderer rend;

    //Type 1: Move Block
    public GameObject moveBlock;
    public bool blockDisappear;
    MoveBlockController moveBlockController;

    void Start()
    {
        leftOpenValue = (leftOpen) ? 1 : -1;
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
            if (blockDisappear)
            {
                moveBlock.SetActive(false);
            }
            rend.material.color = Color.cyan;
            moveBlockController.activated = true;
        }
        else
        {
            //rend.material.color = new Color(255/255f, 0/255f, 0/255f, 255/255f);
            moveBlockController.activated = false;
        }
    }
}
