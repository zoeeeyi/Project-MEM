using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlockController : MonoBehaviour
{
    public bool activated = false;
    MoveBlockState moveState;
    bool open = false;
    bool move = false;

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
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }

        transform.position = globalWaypoints[0];

        //Make two versions of waypoints: Normal and Reversed
        globalWaypointsNormal = globalWaypoints;
        System.Array.Reverse(globalWaypoints);
        globalWaypointsReversed = globalWaypoints;
        globalWaypoints = globalWaypointsNormal;
    }
    void Update()
    {
        if (!activated)
        {
            if(moveState == MoveBlockState.StartPoint)
            {
                move = false;
            }
        }
        if (!activated && open)
        {
            percentBetweenWaypoints = 0;
            globalWaypoints = globalWaypointsReversed;
            move = true;
            open = false;
        }

        if (activated && !open)
        {
            move = true;
            globalWaypoints = globalWaypointsNormal;
        }

        if (move)
        {
            Vector3 velocity = CalculatePlatformMovement();
            transform.Translate(velocity);
        }
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;//when easeAmount = 0, a = 1, there is no easing.
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));//formula that creates easing effect.
    }

    Vector3 CalculatePlatformMovement()
    {
        //When the platform pauses at waypoint, movement is zero
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);//clamp the percentage between 0 and 1.
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if(percentBetweenWaypoints >= 1)
        {
            move = false;
            if (activated) { open = true; }
        }
        /*if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            //If not cyclic, the platform will move back in the reverse order.
            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            //Create a pause when the platform reach a new waypoint
            nextMoveTime = Time.time + waitTime;
        }*/

        return newPos - transform.position;
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
