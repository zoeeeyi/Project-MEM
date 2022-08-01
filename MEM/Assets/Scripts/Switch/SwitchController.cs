using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{

    [Header("Camera Setting")]
    public bool needCameraFocus = false;
    //public Vector3 camTargetPos;

    //Type 1: Move Block
    public bool blockDisappear;

    [Header("Multiple Switch Setting")]
    public List<SwitchController> switchChildrenList;
    List<SwitchChild> switchChildClassList = new List<SwitchChild>();

    [Header("Misc")]
    public bool leftOpen = true;
    [HideInInspector] public int leftOpenValue = 1;
    [HideInInspector] public bool activated = false;
    protected Renderer rend;
    protected Collider2D m_collider;
    protected Animator animator;

    protected virtual void Start()
    {
        if (switchChildrenList.Count == 0)
        {
            leftOpenValue = (leftOpen) ? 1 : -1;

            m_collider = GetComponent<Collider2D>();
            rend = GetComponent<Renderer>();

            animator = GetComponent<Animator>();
        }
        else
        {
            foreach(SwitchController i in switchChildrenList)
            {
                SwitchChild newChild = new SwitchChild(i);
                switchChildClassList.Add(newChild);
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (switchChildrenList.Count == 0)
        {
            if (m_collider.isTrigger) return;
            if (activated)
            {
                animator.SetBool("activated", true);
            }
            else
            {
                animator.SetBool("activated", false);
            }
        } 
        else
        {
            foreach (SwitchChild i in switchChildClassList)
            {
                if (i.CheckChangeStatus())
                {
                    activated = i.lastStatus;
                }
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.tag == "Player"))
        {
            /*m_color.a = 0.5f;
            rend.material.color = m_color;*/
            animator.SetBool("activated", true);
            activated = true;
        }
    }

    class SwitchChild
    {
        public SwitchController switchController;
        public bool lastStatus;
        public bool changeStatus;

        public SwitchChild(SwitchController controller)
        {
            switchController = controller;
            lastStatus = false;
            changeStatus = false;
        }

        public bool CheckChangeStatus()
        {
            if (switchController.activated != lastStatus)
            {
                lastStatus = switchController.activated;
                return true;
            }
            return false;
        }
    }
}
