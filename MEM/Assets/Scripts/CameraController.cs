using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerInputParent playerInputParent;
    public PlayerControllerV2 target;
    public PlayerControllerV2 targetFlipped;
    public Vector2 playerPos;
    public Vector2 padding;
    public Vector2 cameraPadding;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;

    FocusArea focusArea;
    Camera camera;

    float cameraStartSize;
    float widthToCameraSize;
    float heightToCameraSize;
    float focusAreatoCameraSize;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;
    float smoothVelocityX;

    bool lookAheadStopped;

    //Alternative Target
    [Header("Change Target")]
    [HideInInspector] public bool otherTarget = false;
    [HideInInspector] public Vector3 otherTargetPos = Vector3.zero;
    [HideInInspector] public FocusOnOtherTargetState focusOnOtherTargetState = FocusOnOtherTargetState.Back;
    [Range(0, 1)]
    public float changeTargetPauseTime = 0.1f;
    [Range(0,1)]
    public float changeTargetSmoothTime = 0.5f;

    public enum FocusOnOtherTargetState
    {
        Setup,
        Move,
        Pause,
        ReturnMove,
        Back
    }

    void Start()
    {
        camera = GetComponent<Camera>();
        /*Camera actual width and height formula:
        height = 2 * Camera.main.orthographicSize;
        width = height * Camera.main.aspect;*/
        cameraStartSize = camera.orthographicSize;
        heightToCameraSize = 0.5f;
        widthToCameraSize = 0.5f * (1 / camera.aspect);

        focusArea = new FocusArea(target.collider.bounds, targetFlipped.collider.bounds, padding);
        focusAreatoCameraSize = Mathf.Max(focusArea.size.x * widthToCameraSize, focusArea.size.y * heightToCameraSize);
        camera.orthographicSize = Mathf.Max(cameraStartSize, focusAreatoCameraSize);
    }

    void LateUpdate()
    {
        focusArea.Update(target.collider.bounds, targetFlipped.collider.bounds, transform.position);

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        //Camera focus on the other intended target
        if (otherTarget)
        {
            if(focusOnOtherTargetState == FocusOnOtherTargetState.Setup)
            {
                playerPos = focusPosition;
                playerInputParent.freezeMovement = true;
                StartCoroutine(OtherTargetPauseCoroutine(FocusOnOtherTargetState.Move));
                return;
            }

            if (focusOnOtherTargetState == FocusOnOtherTargetState.Move)
            {
                focusPosition = otherTargetPos;
                focusPosition.x = Mathf.SmoothDamp(transform.position.x, focusPosition.x, ref smoothVelocityX, changeTargetSmoothTime);
                focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, changeTargetSmoothTime);
                transform.position = (Vector3)focusPosition + Vector3.forward * -10;
                if (Mathf.Abs(transform.position.x - otherTargetPos.x) < 0.1f)
                {
                    focusOnOtherTargetState = FocusOnOtherTargetState.Pause;
                }
                return;
            }

            if (focusOnOtherTargetState == FocusOnOtherTargetState.Pause) return;

            if (focusOnOtherTargetState == FocusOnOtherTargetState.ReturnMove)
            {
                focusPosition = playerPos;
                focusPosition.x = Mathf.SmoothDamp(transform.position.x, focusPosition.x, ref smoothVelocityX, changeTargetSmoothTime);
                focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, changeTargetSmoothTime);
                transform.position = (Vector3)focusPosition + Vector3.forward * -10;
                if (Mathf.Abs(transform.position.x - playerPos.x) < 0.1f)
                {
                    playerInputParent.freezeMovement = false;
                    otherTarget = false;
                    focusOnOtherTargetState = FocusOnOtherTargetState.Back;
                }
                return;
            }
        }

        if(focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            //Camera will move "lookAheadDstX" when the player touches the edge of focus area
            //But we don't want the camera to move full distance if the player stops moving
            //Only move full distance when player keeps moving in that direction
            if(Mathf.Sign(target.playerInput.getInput().x) == lookAheadDirX && target.playerInput.getInput().x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true; //We only want to do the following calculation once, and then camera completely stops moving
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }
            }
        }

        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        focusAreatoCameraSize = Mathf.Max(focusArea.size.x * widthToCameraSize, focusArea.size.y * heightToCameraSize);
        camera.orthographicSize = Mathf.Max(cameraStartSize, focusAreatoCameraSize);
    }

    public IEnumerator OtherTargetPauseCoroutine(FocusOnOtherTargetState nextState)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(changeTargetPauseTime);
        Time.timeScale = 1;
        focusOnOtherTargetState = nextState;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(focusArea.center, focusArea.size);
    }

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 size;
        public Vector2 velocity;
        Vector2 averageCenter;//this parameter is for the position of focusarea
        Vector2 focusAreaPadding;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Bounds targetBoundsFlipped, Vector2 padding)
        {
            focusAreaPadding = padding;
            averageCenter = (targetBounds.center + targetBoundsFlipped.center) / 2f;

            left = Mathf.Min(targetBounds.center.x, targetBoundsFlipped.center.x) - focusAreaPadding.x;
            right = Mathf.Max(targetBounds.center.x, targetBoundsFlipped.center.x) + focusAreaPadding.x;
            bottom = Mathf.Min(targetBounds.center.y, targetBoundsFlipped.center.y) - focusAreaPadding.y;
            top = Mathf.Max(targetBounds.center.y, targetBoundsFlipped.center.y) + focusAreaPadding.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            size = new Vector2(right - left, top - bottom);
        }

        public void Update(Bounds targetBounds, Bounds targetBoundsFlipped, Vector3 currentPos)
        {
            left = Mathf.Min(targetBounds.center.x, targetBoundsFlipped.center.x) - focusAreaPadding.x;
            right = Mathf.Max(targetBounds.center.x, targetBoundsFlipped.center.x) + focusAreaPadding.x;
            bottom = Mathf.Min(targetBounds.center.y, targetBoundsFlipped.center.y) - focusAreaPadding.y;
            top = Mathf.Max(targetBounds.center.y, targetBoundsFlipped.center.y) + focusAreaPadding.y;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            size = new Vector2(right - left, top - bottom);
            velocity = (Vector3) center - currentPos;
        }
    }
}
