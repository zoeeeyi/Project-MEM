using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;

    public const float skinWidth = 0.015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    [HideInInspector] public float horizontalRaySpacing;
    [HideInInspector] public float verticalRaySpacing;

    [HideInInspector] public BoxCollider2D collider;
    [HideInInspector] public RaycastOrigins raycastOrigins;

    public bool inverseGrav = false;
    [HideInInspector] public int gravityDir = 1;

    //"virtual" will make this method available to be inheritated by the child classes.
    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        if (inverseGrav) gravityDir = -1;
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        if (!inverseGrav)
        {
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        if (inverseGrav)
        {
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.max.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.min.y);
        }
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        //Set ray counts
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //Set ray spacing
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }


    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
