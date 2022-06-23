using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlockController : MonoBehaviour
{
    [HideInInspector] public bool activated = false;
    [HideInInspector] public bool activatedLastStep = false;
    MoveBlockState moveState;
    /*bool open = false;
    bool move = false;*/

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    Vector3[] globalWaypointsNormal;
    Vector3[] globalWaypointsReversed;

    public float speed;
    //public bool cyclic;
    public float waitTime;
    [Range(0, 2)]//clamp easeAmount between 0-2.
    public float easeAmount;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    public enum MoveBlockState
    {
        StartPoint,
        FinishPoint,
        Middle
    }

    void Start()
    {
        //Set global waypoints
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
        if (globalWaypoints.Length < 1) return;
        transform.position = globalWaypoints[0];
        moveState = MoveBlockState.StartPoint;

        //Make two versions of waypoints for further use: Normal and Reversed
        /*globalWaypointsNormal = globalWaypoints;
        System.Array.Reverse(globalWaypoints);
        globalWaypointsReversed = globalWaypoints;
        globalWaypoints = globalWaypointsNormal;*/
    }
    void Update()
    {
        if (globalWaypoints.Length < 1) return;

        Vector3 velocity = Vector3.zero;
        switch (moveState)
        {
            case MoveBlockState.StartPoint:
                if (activated) velocity = CalculatePlatformMovement();
                break;

            case MoveBlockState.FinishPoint:
                if (!activated) velocity = CalculatePlatformMovement();
                break;

            case MoveBlockState.Middle:
                if (activated == activatedLastStep) velocity = CalculatePlatformMovement();
                else if (activated != activatedLastStep)
                {
                    changeDirMidway();
                    velocity = CalculatePlatformMovement();
                }
                break;
        }

        transform.Translate(velocity);
        activatedLastStep = activated;
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
                if (activated) moveState = MoveBlockState.FinishPoint;
                else if (!activated) moveState = MoveBlockState.StartPoint;

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
