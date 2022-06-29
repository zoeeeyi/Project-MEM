using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    [Header("General Settings")]
    public bool bothWay = false;
    public LayerMask passengerMask;

    [Header("Switch Settings")]
    public GameObject switchObject;
    SwitchController switchController;

    [Header("Movement Settings")]
    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    public bool cyclic;
    public float speed;
    public float waitTime;
    [Range(0, 2)]//clamp easeAmount between 0-2.
    public float easeAmount;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    List<PassengerMovementInfo> passengerMovementInfoList;
    Dictionary<Transform, PlayerControllerV2> passengerDic = new Dictionary<Transform, PlayerControllerV2>();

    [Header("Disappear Platform Setting")]
    public bool disappearPlatform = false;
    [Range(0, 10)]
    public float waitToDisappearTime;
    [Range(0, 10)]
    public float disappearTime;
    public bool reappear = false;
    [Range(0, 10)]
    public float reappearTime;
    bool isDisappearing = false;
    float timer = 0;

    Collider2D m_collider;
    Renderer rend;
    Color color;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        try
        {
            switchController = switchObject.GetComponent<SwitchController>();
            switchObject.GetComponent<Collider2D>().isTrigger = true;
        } catch (Exception e) { }

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }

        //Disappearing Platform Settings
        m_collider = GetComponent<Collider2D>();
        rend = GetComponent<Renderer>();
        color = rend.material.color;
        timer = disappearTime;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (!switchController.activated) return;
        } catch(Exception e) { }

        if (isDisappearing)
        {
            /*if (timer >= Mathf.PI)
            {
                timer = 0;
            } else timer += Time.deltaTime * 10;

            color.a = Mathf.Cos(timer);*/
            timer = Mathf.Max(timer - Time.deltaTime, 0);
            color.a = timer / disappearTime;
            rend.material.color = color;
            if (timer <= 0) return;
        }

        UpdateRaycastOrigins();

        Vector3 velocity = Vector3.zero;
        if (globalWaypoints.Length >= 2) velocity = CalculatePlatformMovement();

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
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);//clamp the percentage between 0 and 1.
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
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
            if (!passengerDic.ContainsKey(passenger.transform))
            {
                //map the playercontroller scripts to passenger.transform so we don't need to getcomponent every frame
                passengerDic.Add(passenger.transform, passenger.transform.GetComponent<PlayerControllerV2>());
            }
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                //If the platform isn't bothway and the player have different gravity dir, skip its move step.
                if ((passengerDic[passenger.transform].inverseGrav != inverseGrav))
                {
                    if (!bothWay) continue;
                    //else if (-passenger.velocity.y < passengerDic[passenger.transform].lastDisplacement.y) continue;
                    else passenger.SetVelocity(new Vector3(passenger.velocity.x, -passenger.velocity.y));
                }
                /*else
                {
                    if (passenger.velocity.y < passengerDic[passenger.transform].lastDisplacement.y) continue;
                }*/
                if (passenger.verticalCollision && disappearPlatform && !isDisappearing) StartCoroutine(DisappearCoroutine());
                if (passenger.dontMove) continue;
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

        //Stationary platform, cast small amout of ray to check
        if (velocity == Vector3.zero)
        {
            float rayLength = 2 * skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * gravityDir, rayLength, passengerMask);
                Debug.DrawRay(rayOrigin, gravityDir * Vector2.up * rayLength, Color.red);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        PassengerMovementInfo newPassenger = new PassengerMovementInfo(hit.transform, Vector3.zero, (bothWay || directionY == 1), true, false);
                        newPassenger.SetVerCollision(true);
                        newPassenger.SetDontMove(true);
                        passengerMovementInfoList.Add(newPassenger);
                    }
                }

                if (bothWay)
                {
                    rayOrigin = raycastOrigins.bottomLeft;
                    rayOrigin += Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hitDown = Physics2D.Raycast(rayOrigin, Vector2.down * gravityDir, rayLength, passengerMask);
                    Debug.DrawRay(rayOrigin, gravityDir * Vector2.down * rayLength, Color.red);

                    if (hitDown && hitDown.distance != 0)
                    {
                        if (!movedPassengers.Contains(hitDown.transform))
                        {
                            movedPassengers.Add(hitDown.transform);
                            PassengerMovementInfo newPassenger = new PassengerMovementInfo(hitDown.transform, Vector3.zero, true, true, false);
                            newPassenger.SetVerCollision(true);
                            newPassenger.SetDontMove(true);
                            passengerMovementInfoList.Add(newPassenger);
                        }
                    }
                }
            }

        }

        //Upward moving platforms
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;//the distance only cover the travel length of this frame
            float shortRayLength = 2 * skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                rayLength = Mathf.Abs(velocity.y) + skinWidth;
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;//Ternary
                //Vector2 rayOrigin = raycastOrigins.topLeft;//Ternary
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY * gravityDir, rayLength, passengerMask);
                //RaycastHit2D hitBelowSurface = Physics2D.Raycast(rayOrigin, Vector2.down * directionY * gravityDir, shortRayLength, passengerMask);
                //bool samePassenger = (hit.transform == hitBelowSurface.transform);
                Debug.DrawRay(rayOrigin, 10 * Vector2.up * directionY * gravityDir * rayLength, Color.red);

                if (hit && hit.distance != 0)
                {
                    if ((directionY == -1 && hit.transform.rotation.eulerAngles.x == 180)
                        || (directionY == 1 && hit.transform.rotation.eulerAngles.x == 0))
                    //this if statement is necessary for bothway platform, when the pf is moving down and the "normal" character is below it (vice versa for flipped char)
                    //they will be sucked under the platform without this if statement 
                    //the reason is they won't cast upward ray to detect the platform
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            //the passenger can only be taken horizontally if its on the platform.
                            float pushX = (bothWay || directionY == 1) ? velocity.x : 0;
                            //(hit.distance-skinWidth) is the gap between passenger and the platform
                            //we first close this gap, then move the rest of the distance with "pushY"
                            float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                            PassengerMovementInfo newPassenger = new PassengerMovementInfo(hit.transform, new Vector3(pushX, pushY), (bothWay || directionY == 1), true, false);
                            newPassenger.SetVerCollision(true);
                            passengerMovementInfoList.Add(newPassenger);
                        }
                    }
                }

                if (bothWay)
                {
                    rayLength = 2 * skinWidth;
                    rayOrigin = (directionY == -1) ? raycastOrigins.topLeft : raycastOrigins.bottomLeft;
                    rayOrigin += Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hitDown = Physics2D.Raycast(rayOrigin, Vector2.down * directionY * gravityDir, rayLength, passengerMask);
                    //hitBelowSurface = Physics2D.Raycast(rayOrigin, Vector2.up * directionY * gravityDir, shortRayLength, passengerMask);
                    //samePassenger = (hitDown.transform == hitBelowSurface.transform);
                    Debug.DrawRay(rayOrigin, 10 * Vector2.down * directionY * gravityDir * rayLength, Color.red);

                    if (hitDown && hitDown.distance != 0)
                    {
                        if (!movedPassengers.Contains(hitDown.transform))
                        {
                            movedPassengers.Add(hitDown.transform);
                            float pushX = velocity.x;
                            float pushY = velocity.y;

                            PassengerMovementInfo newPassenger = new PassengerMovementInfo(hitDown.transform, new Vector3(pushX, pushY), true, false, false);
                            newPassenger.SetVerCollision(true);
                            passengerMovementInfoList.Add(newPassenger);
                        }
                    }
                }



                /*if ((hit || hitDown) && (hit.distance != 0) && (hitDown.distance != 0))
                {
                    List<Transform> hitList = new List<Transform>();
                    if (bothWay)
                    {
                        if (!movedPassengers.Contains(hit.transform)) hitList.Add(hit.transform);
                        if (!movedPassengers.Contains(hitDown.transform)) hitList.Add(hitDown.transform);
                    }
                    else
                    {
                        if (!movedPassengers.Contains(hit.transform)) hitList.Add(hit.transform);
                    }

                    foreach (Transform t in hitList)
                    {
                        movedPassengers.Add(t);
                        float pushX = 0;
                        float pushY = 0;
                        if (bothWay)
                        {
                            pushX = velocity.x;
                            pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                        } else
                        {
                            //If the platform is not both way, the passenger can only be taken horizontally if its on the platform.
                            pushX = (directionY == 1) ? velocity.x : 0;
                            //(hit.distance-skinWidth) is the gap between passenger and the platform
                            //we first close this gap, then move the rest of the distance with "pushY"
                            pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                        }
                        passengerMovementInfoList.Add(new PassengerMovementInfo(t, new Vector3(pushX, pushY), true, false, false));
                    }
                }*/


                //Set velocity when collision is detected
            }
        }

        //Horizontally moving platforms (for horizontal collisions)
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
        if ((velocity.y < 0 && !bothWay) || (velocity.y == 0 && velocity.x != 0))
        {
            //only cast a very small ray for detection
            float rayLength = 2 * skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * gravityDir, rayLength, passengerMask);
                //RaycastHit2D hitBelowSurface = Physics2D.Raycast(rayOrigin, Vector2.down * gravityDir, Mathf.Infinity, passengerMask);
                RaycastHit2D hitBelowSurface = Physics2D.Raycast(rayOrigin, Vector2.down * gravityDir, rayLength, passengerMask);
                //sometimes the platform will go through the player and the upward ray will detect the player
                //Meaning this will give player velocity.x
                //Cast ray both upward and downward to detect if the collider is "through" the passenger
                //if the upward and downward detects different objects from "passenger" layer mask, then the collider isn't "through" the passenger
                bool samePassenger = (hit.transform == hitBelowSurface.transform);

                //Set velocity when collision is detected and the passenger is not "through"
                if (hit && !samePassenger && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        PassengerMovementInfo newPassenger = new PassengerMovementInfo(hit.transform, new Vector3(pushX, pushY), true, false, false);
                        newPassenger.SetVerCollision(true);
                        passengerMovementInfoList.Add(newPassenger);
                    }
                }

                if (bothWay)
                {
                    rayOrigin = raycastOrigins.bottomLeft + Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hitDown = Physics2D.Raycast(rayOrigin, Vector2.down * gravityDir, rayLength, passengerMask);
                    hitBelowSurface = Physics2D.Raycast(rayOrigin, Vector2.up * gravityDir, rayLength, passengerMask);
                    samePassenger = (hitDown.transform == hitBelowSurface.transform);

                    if (hitDown && !samePassenger && hitDown.distance != 0)
                    {
                        if (!movedPassengers.Contains(hitDown.transform))
                        {
                            movedPassengers.Add(hitDown.transform);
                            float pushX = velocity.x;
                            float pushY = velocity.y;

                            PassengerMovementInfo newPassenger = new PassengerMovementInfo(hitDown.transform, new Vector3(pushX, pushY), true, false, false);
                            newPassenger.SetVerCollision(true);
                            passengerMovementInfoList.Add(newPassenger);
                        }
                    }
                }


                /*if ((hit || hitDown) && !samePassenger && (hit.distance != 0) && (hitDown.distance != 0))
                {
                    List<Transform> hitList = new List<Transform>();
                    if (bothWay)
                    {
                        if (!movedPassengers.Contains(hit.transform)) hitList.Add(hit.transform);
                        if (!movedPassengers.Contains(hitDown.transform)) hitList.Add(hitDown.transform);
                    }
                    else
                    {
                        if (!movedPassengers.Contains(hit.transform)) hitList.Add(hit.transform);
                    }

                    foreach (Transform t in hitList)
                    {
                        movedPassengers.Add(t);
                        float pushX = velocity.x;
                        float pushY = velocity.y;
                        passengerMovementInfoList.Add(new PassengerMovementInfo(t, new Vector3(pushX, pushY), true, false, false));
                    }
                }*/

            }

        }
    }

    IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(waitToDisappearTime);
        isDisappearing = true;
        yield return new WaitForSeconds(disappearTime);
        m_collider.enabled = false;
        if (reappear) StartCoroutine(ReappearCoroutine());
    }

    IEnumerator ReappearCoroutine()
    {
        yield return new WaitForSeconds(reappearTime);
        m_collider.enabled = true;
        isDisappearing = false;
        color.a = 1;
        rend.material.color = color;
        timer = disappearTime;
    }

    struct PassengerMovementInfo
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;
        public bool overwritePlatformPush;
        public bool verticalCollision;
        public bool dontMove;

        public PassengerMovementInfo(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform, bool _overwritePlatformPush)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
            overwritePlatformPush = _overwritePlatformPush;
            verticalCollision = false; //for disappearing platform
            dontMove = false; //for disappearing platform
        }

        public void SetVelocity(Vector3 _velocity)
        {
            velocity = _velocity;
        }

        public void SetVerCollision(bool _verCollision)
        {
            verticalCollision = _verCollision;
        }

        public void SetDontMove(bool _dontMove)
        {
            dontMove = _dontMove;
        }
    }

    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.blue;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size * gravityDir, globalWaypointPos + Vector3.up * gravityDir * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
