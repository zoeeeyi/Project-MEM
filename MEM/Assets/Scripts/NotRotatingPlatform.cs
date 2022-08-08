using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotRotatingPlatform : RaycastController
{
    Vector3 lastPosition;
    Vector3 velocity;

    public LayerMask passengerMask;
    RotatingKiller parent;

    List<PassengerMovementInfo> passengerMovementInfoList;
    Dictionary<Transform, PlayerControllerV2> passengerDic = new Dictionary<Transform, PlayerControllerV2>();


    public override void Start()
    {
        base.Start();
        parent = GetComponentInParent<RotatingKiller>();
        lastPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(0, 0, -parent.hiddenRotationSpeed);
        velocity = transform.position - lastPosition;

        UpdateRaycastOrigins();

        CalculatePassengerMovement(velocity);
        MovePassengers(true);

        lastPosition = transform.position;
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
                passengerDic[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform, passenger.overwritePlatformPush);
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovementInfoList = new List<PassengerMovementInfo>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Cast small ray on both sides to detect players
        float rayLength = 2 * skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
            Debug.DrawRay(rayOrigin, Vector2.up * rayLength * 10, Color.red);

            rayOrigin = raycastOrigins.bottomLeft + Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hitDown = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, passengerMask);
            Debug.DrawRay(rayOrigin, Vector2.down * rayLength * 10, Color.red);

            if (hit && hit.distance != 0)
            {
                if (hit.transform.localEulerAngles.z == 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        //float pushY = 0;
                        //float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                        float pushY;
                        if (directionY == 1) pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                        else pushY = velocity.y;

                        PassengerMovementInfo newPassenger = new PassengerMovementInfo(hit.transform, new Vector3(pushX, pushY), true, true, false);
                        passengerMovementInfoList.Add(newPassenger);
                    }
                }
            }

            if (hitDown && hitDown.distance != 0)
            {
                if (hitDown.transform.localEulerAngles.z == 180)
                {
                    if (!movedPassengers.Contains(hitDown.transform))
                    {
                        movedPassengers.Add(hitDown.transform);
                        float pushX = velocity.x;
                        //float pushY = 0;
                        float pushY;
                        if (directionY == -1) pushY = velocity.y - (hitDown.distance - skinWidth) * directionY;
                        else pushY = velocity.y;

                        PassengerMovementInfo newPassenger = new PassengerMovementInfo(hitDown.transform, new Vector3(pushX, -pushY), true, true, false);
                        passengerMovementInfoList.Add(newPassenger);
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

        public void SetVelocity(Vector3 _velocity)
        {
            velocity = _velocity;
        }
    }

}
