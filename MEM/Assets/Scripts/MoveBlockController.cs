using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
public class MoveBlockController : MonoBehaviour
{
    [HideInInspector] public bool activated = false;
    [HideInInspector] public bool activatedLastStep = false;
    MoveBlockState moveState;

    [Header("Switch Setting")]
    public bool blockDisappear = false;
    public GameObject switchObject;
    SwitchController switchController;

    [Header("Movement Settings")]
    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    Vector3 middlePoint;

    public float speed;
    public float waitTime;
    [Range(0, 2)]//clamp easeAmount between 0-2.
    public float easeAmount;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    //Camera Setting (optional)
    CameraController cameraController;

    //Animation
    Animator animator;

    //Audio
    AudioManager audioManager;

    public enum MoveBlockState
    {
        StartPoint,
        FinishPoint,
        Middle,
        Done
    }


    private void Awake()
    {
        Collider2D switchCollider = switchObject.GetComponent<Collider2D>();
        switchController = switchObject.GetComponent<SwitchController>();
        if (blockDisappear) switchCollider.isTrigger = true;
    }

    void Start()
    {
        //Run if we need alternative camera focus
        if(switchController.needCameraFocus) cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        //Set global waypoints
        globalWaypoints = new Vector3[localWaypoints.Length];
        Vector3 locationSum = Vector3.zero;

        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
            locationSum += globalWaypoints[i];
        }

        middlePoint = locationSum / 2;

        if (globalWaypoints.Length < 2) return;

        transform.position = globalWaypoints[0];
        moveState = MoveBlockState.StartPoint;

        //Make two versions of waypoints for further use: Normal and Reversed
        /*globalWaypointsNormal = globalWaypoints;
        System.Array.Reverse(globalWaypoints);
        globalWaypointsReversed = globalWaypoints;
        globalWaypoints = globalWaypointsNormal;*/
    }

    private void FixedUpdate()
    {
        if (cameraController == null && switchController.needCameraFocus) cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    void Update()
    {
        if (moveState == MoveBlockState.Done) return;
        if (blockDisappear && switchController.activated)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Renderer>().enabled = false;
            return;
        }
        if (blockDisappear && !switchController.activated)
        {
            GetComponent<Collider2D>().enabled = true;
            GetComponent<Renderer>().enabled = true;
            return;
        }
        if (globalWaypoints.Length < 2) return;

        //Camera reFocus
        try
        {
            if (switchController.needCameraFocus && switchController.activated)
            {
                switchController.needCameraFocus = false;
                cameraController.otherTarget = true;
                cameraController.otherTargetPos = middlePoint;
                cameraController.focusOnOtherTargetState = CameraController.FocusOnOtherTargetState.Setup;
                return;
            }

            if (cameraController.focusOnOtherTargetState == CameraController.FocusOnOtherTargetState.Move) return;

            if (moveState == MoveBlockState.FinishPoint && cameraController.focusOnOtherTargetState == CameraController.FocusOnOtherTargetState.Pause)
            {
                cameraController.StartCoroutine(cameraController.OtherTargetPauseCoroutine(CameraController.FocusOnOtherTargetState.ReturnMove));
                moveState = MoveBlockState.Done;
            }
        }
        catch (Exception e) { }

        Vector3 velocity = Vector3.zero;
        switch (moveState)
        {
            case MoveBlockState.StartPoint:
                if (switchController.activated)
                {
                    velocity = CalculatePlatformMovement();
                    audioManager.playAudioClip("MovingBlock");
                }
                break;

            case MoveBlockState.FinishPoint:
                if (!switchController.activated)
                {
                    velocity = CalculatePlatformMovement();
                    audioManager.playAudioClip("MovingBlock");
                }
                break;

            case MoveBlockState.Middle:
                if (switchController.activated == activatedLastStep) velocity = CalculatePlatformMovement();
                else if (switchController.activated != activatedLastStep)
                {
                    changeDirMidway();
                    velocity = CalculatePlatformMovement();
                    audioManager.playAudioClip("MovingBlock");
                }
                break;
        }

        transform.Translate(velocity);
        if(moveState == MoveBlockState.Middle)
        {
            animator.SetTrigger("Middle");
        } else if (moveState == MoveBlockState.FinishPoint)
        {
            animator.SetTrigger("Finish");
        }
        activatedLastStep = switchController.activated;
    }

    //Do if we change direction middle of the way
    void changeDirMidway()
    {
        System.Array.Reverse(globalWaypoints);
        fromWaypointIndex = globalWaypoints.Length - 1 - (fromWaypointIndex + 1);
        percentBetweenWaypoints = 1 - percentBetweenWaypoints;
    }

    Vector3 CalculatePlatformMovement()
    {
        //When the platform pauses at waypoint, movement is zero
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;//Which waypoint the object just passed
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;//Next waypoint

        //Calculate the percentage we have traveled between waypoints (value subject to the order of waypoints)
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);//clamp the percentage between 0 and 1.
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        //Get new position
        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        //Move to next waypoint
        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            //When we reach the last waypoint
            if (fromWaypointIndex == globalWaypoints.Length - 1)
            {
                if (switchController.activated) moveState = MoveBlockState.FinishPoint;
                else if (!switchController.activated) moveState = MoveBlockState.StartPoint;

                //Reset
                System.Array.Reverse(globalWaypoints);
                fromWaypointIndex = 0;
                percentBetweenWaypoints = 0;
                return Vector3.zero;
            }

            //Create a pause when the platform reach a new waypoint
            nextMoveTime = Time.time + waitTime;
        }

        moveState = MoveBlockState.Middle;
        return newPos - transform.position;
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;//when easeAmount = 0, a = 1, there is no easing.
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));//formula that creates easing effect.
    }

    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
