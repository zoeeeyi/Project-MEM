using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputParent : MonoBehaviour
{
    [Space]
    [Header("Move")]
    //Horizontal parameters
    public float moveSpeed = 6;
    public float xAccelerationTimeGrounded = 0.1f;
    public float xAccelerationTimeAir = 0.2f;

    [Space]
    [Header("Jump")]
    [HideInInspector] public float gravity;
    [HideInInspector]public float maxJumpVelocity;
    [HideInInspector] public float minJumpVelocity;
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    //It's more intuitive to set jump height and time than to set gravity and initial velocity.
    //We can calculate gravity and jumpVelocity with these two data.

    [Space]
    [Header("Wall Jump")]
    //Wall jump parameters
    public bool wallJumpActivate = true;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    //Specifically for wall leaping, we want to temporarily pause player on each leap so that they don't slide down
    public float wallStickTime = 0.25f;
    //Wall sliding parameter
    //public float wallSlideSpeedMax = 3;
    public float wallSlideGravBuffer = 0.5f;

    [Space]
    [Header("Center Dash")]
    public float centerDashVelocityX = 10;
    [HideInInspector] public float characterXGap;

    [HideInInspector] public Vector3 posCharacter, posCharacterFlipped;
    [HideInInspector] public Vector3 characterCenter;

    [Space]
    [Header("Misc")]
    public float maxCharacterXGap;
    public List<string> HorizontalCollisionExemptions = new List<string>();
    public List<string> VerticalCollisionExemptions = new List<string>();
    public List<string> Killers = new List<string>();

    SavePointController savePointController;

    [HideInInspector]
    public enum PlayerState
    {
        Align,
        Normal,
        CenterDash,
        NearlyBeyondXGap,
        BeyondXGap
    }

    [HideInInspector] public PlayerState state;

    private void Start()
    {
        gravity = -2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2);
        //kinetic movement equation [v0t + (1/2)gt^2] = s. Here we can assume v0 = 0 because the time is exact the same as throwing object down.
        maxJumpVelocity = (-1) * gravity * timeToJumpApex;
        //vt = vo + gt
        minJumpVelocity = -Mathf.Sign(gravity) * Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        state = PlayerState.Normal;

        savePointController = GameObject.Find("SavePointController").GetComponent<SavePointController>();
        transform.position = savePointController.lastSavePos;
    }

    void FixedUpdate()
    {
        characterXGap = posCharacter.x - posCharacterFlipped.x;

        if (Input.GetKeyDown(KeyCode.E))
        {
            characterCenter = (posCharacter + posCharacterFlipped) / 2;
            state = PlayerState.CenterDash;
        }

        /*if (characterXGap == 0)
        {
            state = PlayerState.Normal;
        }*/

        if (characterXGap <= 0.6f * maxCharacterXGap) state = PlayerState.Normal;
        else if (characterXGap > 0.6f * maxCharacterXGap && characterXGap <= maxCharacterXGap) state = PlayerState.NearlyBeyondXGap;
        else if (characterXGap > maxCharacterXGap) state = PlayerState.BeyondXGap;   
    }
}
