using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    [Header("General Settings")]
    public bool bothWay = false;
    public LayerMask passengerMask;

    [Header("Movement Settings")]
    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    public float speed;
    public bool cyclic;
    public float waitTime;
    [Range(0,2)]//clamp easeAmount between 0-2.
    public float easeAmount;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    List<PassengerMovementInfo> passengerMovementInfoList;
    Dictionary<Transform, PlayerControllerV2> passengerDic = new Dictionary<Transform, PlayerControllerV2>();

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i=0; i<localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        velocity.y *= gravityDir;

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);

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
        percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);//clamp the percentage between 0 and 1.
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if(percentBetweenWaypoints >= 1)
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
        }

        return newPos - transform.position;
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovementInfo passenger in passengerMovementInfoList)
        {
            PlayerControllerV2 playerController = passenger.transform.GetComponent<PlayerControllerV2>();
            if (playerController.inverseGrav != inverseGrav) continue;
            if (!passengerDic.ContainsKey(passenger.transform))
            {
                //map the playercontroller scripts to passenger.transform so we don't need to getcomponent every frame
                passengerDic.Add(passenger.transform, passenger.transform.GetComponent<PlayerControllerV2>());
            }
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passengerDic[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform, passenger.overwritePlatformPush);
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        /*create a hashset to quickly remember passengers that have already been moved in this frame so we don't
        move them again because of multiple ray casts*/
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovementInfoList = new List<PassengerMovementInfo>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //vertically moving platforms
        if(velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;//the distance only cover the travel length of this frame

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;//Ternary
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY * gravityDir, rayLength, passengerMask);
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength * gravityDir, Color.green);

                //Set velocity when collision is detected
                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        //the passenger can only be taken horizontally if its on the platform.
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        //(hit.distance-skinWidth) is the gap between passenger and the platform
                        //we first close this gap, then move the rest of the distance with "pushY"
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        passengerMovementInfoList.Add(new PassengerMovementInfo(hit.transform, new Vector3(pushX, pushY), directionY == 1, true, false));
                    }
                }
            }
        }

        //Horizontally moving platforms
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i) * gravityDir;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.green);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = 0;

                        passengerMovementInfoList.Add(new PassengerMovementInfo(hit.transform, new Vector3(pushX, pushY), false, true, true));
                    }
                }
            }
        }

        //Passengers on top of a horizontally or downward moving platform
        if (velocity.y < 0 || (velocity.y == 0 && velocity.x != 0))
        {
            //only cast a very small ray for detection
            float rayLength = 2 * skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * gravityDir, rayLength, passengerMask);
                RaycastHit2D hitDown = Physics2D.Raycast(rayOrigin, Vector2.down * gravityDir, Mathf.Infinity, passengerMask);
                //sometimes the platform will go through the player and the upward ray will detect the player
                //Meaning this will give player velocity.x
                //Cast ray both upward and downward to detect if the collider is "through" the passenger
                //if the upward and downward detects different objects from "passenger" layer mask, then the collider isn't "through" the passenger
                bool samePassenger = (hit.transform == hitDown.transform);

                //Set velocity when collision is detected
                if (hit && !samePassenger && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovementInfoList.Add(new PassengerMovementInfo(hit.transform, new Vector3(pushX, pushY), true, false, false));
                    }
                }
            }

        }
    }
    struct PassengerMovementInfo
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;
        public bool overwritePlatformPush;

        public PassengerMovementInfo(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform, bool _overwritePlatformPush)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
            overwritePlatformPush = _overwritePlatformPush;
        }
    }

    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.3f;

            for (int i=0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size * gravityDir, globalWaypointPos + Vector3.up * gravityDir * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
