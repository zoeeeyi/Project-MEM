using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControllerV2 : RaycastController
{
    [SerializeField] float maxClimbAngle = 80;
    [SerializeField] float maxDescendAngle = 75;

    GameManager gameManager;
    Animator animator;
    AudioManager audioManager;

    public CollisionInfo collisionInfo;
    public PlayerInput playerInput;

    public Vector3 lastDisplacement = Vector3.zero;

    //Dictionary<Transform, PlatformController> movePlatformDic = new Dictionary<Transform, PlatformController>();

    public override void Start()
    {
        base.Start();
        playerInput = GetComponent<PlayerInput>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        animator = GetComponent<Animator>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        collisionInfo.faceDir = 1;
    }

    /*public void Move(Vector3 displacement, bool standingOnPlatform, bool overwritePlatformPush)
    {
        Move(displacement, Vector2.zero, standingOnPlatform, overwritePlatformPush);
    }*/


    public void Move(Vector3 displacement, bool standingOnPlatform = false, bool overwritePlatformPush = false, bool needAnimation = false)
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();

        //When platforms do a horizontal push on the player, the pushY is 0 so player won't castRay below
        //We can overwrite this with original player inputs
        if (overwritePlatformPush)
        {
            displacement.y = playerInput.displacement.y;
        }

        if(displacement.y < 0)
        {
            DescendSlope(ref displacement);
        }

        if (displacement.x != 0)
        {
            collisionInfo.faceDir = (int)Mathf.Sign(displacement.x);
        }
        HorizontalCollisions(ref displacement);
        //"ref" will pass through the velocity variable to Move() method from VerticalCollision()

        if(displacement.y != 0)
        {
            VerticalCollisions(ref displacement);
            //in case there is a new slope while we are climbing the slope, we need to detect it beforehand
            if (collisionInfo.climbingSlope)
            {
                SlopeTransition(ref displacement);
            }
        }

        transform.Translate(displacement);
        lastDisplacement = displacement;

        if (standingOnPlatform)
        {
            collisionInfo.below = true;
        }

        if (needAnimation) SetAnimation(displacement);

    }

    void SetAnimation(Vector3 displacement)
    {
        animator.SetFloat("MoveX", Mathf.Abs(displacement.x));
        animator.SetFloat("MoveY", displacement.y);
    }

    void HorizontalCollisions(ref Vector3 displacement)
    {
        float directionX = collisionInfo.faceDir;
        float rayLength = Mathf.Abs(displacement.x) + skinWidth;

        //when the player is not moving, it cast a very small ray just to detect wall.
        if (Mathf.Abs(displacement.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i=0; i<horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            //Raycasts do not use local vectors. So anything related to vertical raycast should be multiplied by gravityDir
            rayOrigin += Vector2.up * (horizontalRaySpacing * i) * gravityDir;
            //Detect if there is an obstacle
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                //Collision Exemptions
                if (playerInput.parent.HorizontalCollisionExemptions.Contains(hit.collider.tag))
                {
                    continue;
                }

                //If we hit killers
                /*if (playerInput.parent.Killers.Contains(hit.collider.tag))
                {
                    gameManager.gameOver = true;
                }*/

                if (hit.distance == 0)
                {
                    continue;
                }

                /*if(hit.collider.tag == "MovePlatform")
                {
                    try
                    {
                        if (!movePlatformDic.ContainsKey(hit.transform))
                        {
                            movePlatformDic.Add(hit.transform, hit.transform.GetComponent<PlatformController>());
                        }
                        if ((movePlatformDic[hit.transform].inverseGrav != inverseGrav) && (!movePlatformDic[hit.transform].bothWay)) continue;
                    }
                    catch (NullReferenceException e) { }
                }*/

                if (hit.collider.tag == "Switch")
                {
                    SwitchController switchController = hit.collider.GetComponent<SwitchController>();
                    if (!hit.collider.isTrigger)
                    {
                        if (directionX == 1 * switchController.leftOpenValue)
                        {
                            switchController.activated = true;
                            continue;
                        }
                        else
                        {
                            switchController.activated = false;
                            continue;
                        }
                    }
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up * gravityDir);
                //each frame, start checking with the first ray if the object can climb the slope.
                if (i == 0 && (slopeAngle <= maxClimbAngle))
                {
                    //New slope: if the slope angle is not equal to the previous one
                    if (slopeAngle != collisionInfo.slopeAngleOld)
                    {
                        collisionInfo.descendingSlope = false;
                        collisionInfo.slopeAngle = slopeAngle;
                        //move to the edge of new slope instead of immediately climb
                        displacement.x = (hit.distance - skinWidth) * directionX;
                    }
                    else
                    {
                        ClimbSlope(ref displacement, slopeAngle);
                    }
                }

                //If the object is not climbing any slope or the slope is too steep, act normal.
                if (!collisionInfo.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    displacement.x = Mathf.Min(Mathf.Abs(displacement.x), (hit.distance - skinWidth)) * directionX;
                    rayLength = Mathf.Min(Mathf.Abs(displacement.x) + skinWidth, hit.distance);

                    if (collisionInfo.climbingSlope)
                    {
                        displacement.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(displacement.x);
                    }

                    collisionInfo.left = (directionX == -1); //equivalent to if(directionY == -1) collision.below = true;
                    collisionInfo.right = (directionX == 1);
                }
            }

            //------------------------------------------------------------------//

            //Cast a small ray to check opposite direction
            rayLength = 2 * skinWidth;
            rayOrigin = (directionX == 1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            //Raycasts do not use local vectors. So anything related to vertical raycast should be multiplied by gravityDir
            rayOrigin += Vector2.up * (horizontalRaySpacing * i) * gravityDir;
            //Detect if there is an obstacle
            hit = Physics2D.Raycast(rayOrigin, Vector2.left * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.left * directionX * rayLength, Color.red);


            if (hit)
            {
                if(displacement.x == 0) displacement.x = (hit.distance - skinWidth) * -directionX;

                //displacement.x = Mathf.Max(Mathf.Abs(displacement.x), (hit.distance - skinWidth)) * -directionX;

                /*float slopeAngle = Vector2.Angle(hit.normal, Vector2.up * gravityDir);
                //each frame, start checking with the first ray if the object can climb the slope.
                if (i == 0 && (slopeAngle <= maxClimbAngle))
                {

                    //New slope: if the slope angle is not equal to the previous one
                    if (slopeAngle != collisionInfo.slopeAngleOld)
                    {
                        collisionInfo.descendingSlope = false;
                        collisionInfo.slopeAngle = slopeAngle;
                        //move to the edge of new slope instead of immediately climb
                        displacement.x = (hit.distance - skinWidth) * directionX;
                    }
                    else
                    {
                        ClimbSlope(ref displacement, slopeAngle);
                    }
                }*/
            }

        }
    }


    void VerticalCollisions(ref Vector3 displacement)
    {
        float directionY = Mathf.Sign(displacement.y);
        float rayLength = Mathf.Abs(displacement.y) + skinWidth;//the distance only cover the travel length of this frame

        for (int i = 0; i < verticalRayCount; i++)
        {
            //Cast i th ray
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;//Ternary
            rayOrigin += Vector2.right * (verticalRaySpacing * i + displacement.x);

            //Detect collision
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY * gravityDir, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength * gravityDir, Color.red);

            //Set velocity when collision is detected
            if (hit)
            {
                //Collision Exemptions
                if (playerInput.parent.VerticalCollisionExemptions.Contains(hit.collider.tag))
                {
                    continue;
                }

                //If we hit killers
                /*if (playerInput.parent.Killers.Contains(hit.collider.tag))
                {
                    gameManager.gameOver = true;
                }*/

                if (hit.collider.tag == "MovePlatform")
                {
                    /*try
                    {
                        if (!movePlatformDic.ContainsKey(hit.transform))
                        {
                            movePlatformDic.Add(hit.transform, hit.transform.GetComponent<PlatformController>());
                        }
                        if ((movePlatformDic[hit.transform].inverseGrav != inverseGrav) && (!movePlatformDic[hit.transform].bothWay)) continue;
                    } catch (NullReferenceException e) { }
                    /*try
                    {
                        PlatformController platformController = hit.collider.GetComponent<PlatformController>();
                        if ((platformController.inverseGrav != inverseGrav)
                            && (!platformController.bothWay)){
                            continue;
                        }
                    }catch(NullReferenceException e) { }*/

                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }

                    if (playerInput.getInput().y == -1)
                    {
                        collisionInfo.fallThroughPlatform = hit.collider;
                        continue;
                    }
                    //We release down button at instant, but falling through may take some time.
                    //Without following check, game wouldn't know that the player is still falling through, sometimes will cause jigering.
                    //We need to keep track if we are still falling through the same platform.
                    if (collisionInfo.fallThroughPlatform == hit.collider)
                    {
                        continue;
                    }
                } else if(collisionInfo.fallThroughPlatform != null)
                {
                    collisionInfo.fallThroughPlatform = null;
                }
                displacement.y = (hit.distance - skinWidth) * directionY;
                //when the object touches the collider, (hit.distance - skinWidth) = 0. The velocity is set to 0.
                rayLength = hit.distance;

                if (collisionInfo.climbingSlope)
                {
                    displacement.x = displacement.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(displacement.x);
                }

                collisionInfo.below = (directionY == -1);
                collisionInfo.above = (directionY == 1);

            }
        }
    }

    void ClimbSlope(ref Vector3 displacement, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(displacement.x);
        float yClimbVelocity = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        //make an exception for jumping.
        if(displacement.y <= yClimbVelocity)
        {
            displacement.y = yClimbVelocity;
            displacement.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(displacement.x);
            collisionInfo.below = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector3 displacement)
    {
        float directionX = Mathf.Sign(displacement.x);
        //when descending, we need to reverse the direction of ray origins to detect the collision
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up * gravityDir, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up * gravityDir);
            //check if the object is on a flat surface or the descend angle is too high
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                //hit.normal will point "outward" a slope. If the moving direction is the same as hit.normal, the object is descending.
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    //if the y distance to the slope down below is smaller than the distance we need to move down, we need to do the conversion
                    if(hit.distance-skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(displacement.x))
                    {
                        float moveDistance = Mathf.Abs(displacement.x);
                        float yDescendVelocity = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        displacement.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(displacement.x);
                        displacement.y -= yDescendVelocity;

                        collisionInfo.slopeAngle = slopeAngle;
                        collisionInfo.descendingSlope = true;
                        collisionInfo.below = true;
                    }
                }
            }
        }
    }

    void SlopeTransition(ref Vector3 displacement)
    {
        float directionX = Mathf.Sign(displacement.x);
        float rayLength = Mathf.Abs(displacement.x) + skinWidth;
        Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * displacement.y * gravityDir;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up * gravityDir);
            if(slopeAngle != collisionInfo.slopeAngle)
            {
                displacement.x = (hit.distance - skinWidth) * directionX;
                collisionInfo.slopeAngle = slopeAngle;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Endpoint"))
        {
            EndPoint endPoint = other.GetComponent<EndPoint>();
            endPoint.reached = true;
            gameManager.endPointReached += 1;
            //this.gameObject.SetActive(false);
        }

        if (playerInput.parent.Killers.Contains(other.tag))
        {
            if (other.tag == "DeathBound") audioManager.playAudioClip("DeathFall");
            if (other.tag == "Enemy") audioManager.playAudioClip("DeathHit");
            if (other.tag == "RotatingKiller") audioManager.playAudioClip("DeathHit");
            gameManager.ChangeGameStateTo(GameManager.GameStates.GameOver);
        }

        /*if (other.gameObject.CompareTag("Switch"))
        {
            SwitchController switchController = other.GetComponent<SwitchController>();
            switchController.activated = true;
        }*/
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public int faceDir;

        public Collider2D fallThroughPlatform;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}