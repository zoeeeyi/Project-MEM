using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerControllerV2))]
public class PlayerInput : MonoBehaviour
{
    float velocityXSmoothing;

    bool isJumping = false;

    float timeToWallUnstick;

    int centerDashDir;

    Renderer rend;

    [Space]
    [Header("Misc")]
    public bool inverseGravity = false;
    Vector2 input;
    Vector3 velocity;
    float gravity;
    [HideInInspector] public Vector3 displacement;
    PlayerControllerV2 controller;
    [HideInInspector] public PlayerInputParent parent;
    Animator animator;

    PlayerInputParent.PlayerState state;

    void Start()
    {
        controller = GetComponent<PlayerControllerV2>();
        parent = GetComponentInParent<PlayerInputParent>();
        rend = GetComponent<Renderer>();
        animator = GetComponent<Animator>();

        //If we reload the scene, we read the position from the parent, which is the last saving point
        if (!inverseGravity)
        {
            transform.position = parent.posCharacter;
        }
        else
        {
            transform.position = parent.posCharacterFlipped;
        }
    }

    void FixedUpdate()
    {
        //Send the real time location to parent
        if (!inverseGravity)
        {
            parent.posCharacter = transform.position;
        }
        else
        {
            parent.posCharacterFlipped = transform.position;
        }
    }

    void Update()
    {
        //Set state from parent
        state = parent.state;

        if (parent.state == PlayerInputParent.PlayerState.Normal) rend.material.color = Color.white;
        if (parent.state == PlayerInputParent.PlayerState.NearlyBeyondXGap) rend.material.color = Color.green;
        if (parent.state == PlayerInputParent.PlayerState.BeyondXGap) rend.material.color = Color.blue;


        if (parent.freezeMovement)
        {
            velocity.x = 0;
            return;
        }

        //Get inputs
        //float xInput = (Input.GetKey(KeyCode.D)?1:0) + (Input.GetKey(KeyCode.A)?-1:0);
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(input.x > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        } 
        else if (input.x < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }

        bool isGrounded = (controller.collisionInfo.above || controller.collisionInfo.below);

        if (isGrounded) animator.SetBool("isJumping", false);

        //Calculate movement data
        float targetVelocityX = input.x * parent.moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (isGrounded) ? parent.xAccelerationTimeGrounded : parent.xAccelerationTimeAir);

        //Wall jump
        int wallDirX = (controller.collisionInfo.left) ? -1 : 1;

        //Wall sliding
        bool wallSliding = false;
        //if we are sliding on the wall, vertical speed is reduced (max 3)
        if ((controller.collisionInfo.left || controller.collisionInfo.right) && (!controller.collisionInfo.below) && velocity.y < 0)
        {
            wallSliding = true;

            /*if(velocity.y < -parent.wallSlideSpeedMax)
            {
                velocity.y = -parent.wallSlideSpeedMax;
            }*/

            if(timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if(input.x != wallDirX && (input.x != 0))
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = parent.wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = parent.wallStickTime;
            }
        }

        //Reset vertical velocity to 0 when on the ground or touching the ceiling
        if (isGrounded)
        {
            velocity.y = 0;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("isJumping", true);

            if (wallSliding)
            {
                //Jump climb the wall
                if (parent.wallJumpActivate)
                {
                    if (wallDirX == input.x)
                    {
                        velocity.x = -wallDirX * parent.wallJumpClimb.x;
                        velocity.y = parent.wallJumpClimb.y;
                    }
                    else
                    {
                        velocity.x = -wallDirX * parent.wallLeap.x;
                        velocity.y = parent.wallLeap.y;
                    }
                }
                //Jump off the wall
                if(input.x == 0)
                {
                    velocity.x = -wallDirX * parent.wallJumpOff.x;
                    velocity.y = (parent.wallJumpActivate) ? parent.wallJumpClimb.y : 0;
                }
                //Leap between two walls
            }
            if (isGrounded)
            {
                velocity.y = parent.maxJumpVelocity;
                isJumping = true;
            }
        }

        /*during jumping, if we release the space bar before player reaches the max height
        we "terminate" the jump early by setting velocity.y to a small level*/
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isJumping)
            {
                if (velocity.y > parent.minJumpVelocity)
                {
                    velocity.y = parent.minJumpVelocity;
                }
            }
            isJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (((!inverseGravity) && (parent.characterXGap > 0))//normal character on right.
                ||((inverseGravity) && (parent.characterXGap < 0))) //flipped character on right.
            {
                centerDashDir = -1;
            } else if (((!inverseGravity) && (parent.characterXGap < 0))//normal character on left.
                || ((inverseGravity) && (parent.characterXGap > 0)))//flipped character on left.
            {
                centerDashDir = 1;
            } else
            {
                centerDashDir = 0;
            }

            //velocity.x = 0;
        }

        if ((state == PlayerInputParent.PlayerState.CenterDash) && (controller.collisionInfo.left || controller.collisionInfo.right))
        {
            velocity.x = 0;
            parent.state = PlayerInputParent.PlayerState.Normal;
        }

        switch (state)
        {
            /*case PlayerInputParent.PlayerState.CenterDash:
                velocity.x = Mathf.SmoothDamp(velocity.x, centerDashDir * parent.centerDashVelocityX, ref velocityXSmoothing, parent.xAccelerationTimeAir);
                float DisplacementX = velocity.x * Time.deltaTime;
                float nextPos = transform.position.x + DisplacementX;
                if (centerDashDir * (nextPos - parent.characterCenter.x) > 0)
                {
                    Debug.Log("Trigger");
                    displacement.x = parent.characterCenter.x - transform.position.x;
                    displacement.y = 0;
                }
                else
                {
                    displacement.x = DisplacementX;
                    displacement.y = 0;
                }
                break;*/

            default:
                //velocity.x = input.x * moveSpeed;
                displacement.x = velocity.x * Time.deltaTime;
                float yInitialVelocity = velocity.y;
                gravity = (wallSliding) ? (parent.wallSlideGravBuffer * parent.gravity) : parent.gravity;
                velocity.y += gravity * Time.deltaTime;
                displacement.y = (Mathf.Pow(velocity.y, 2) - Mathf.Pow(yInitialVelocity, 2)) / (2 * gravity);
                break;
        }

        controller.Move(displacement, false, false, true);
    }

    public Vector2 getInput()
    {
        return this.input;
    }
}