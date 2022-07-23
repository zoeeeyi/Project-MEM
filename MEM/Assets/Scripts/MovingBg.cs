using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingBg : MonoBehaviour
{
    public Camera cam;
    [Range(-0.001f, -0.0001f)]
    public float vModifier = -0.0002f;

    Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        try
        {
            velocity = cam.velocity * vModifier;
            transform.Translate(new Vector3(velocity.x, 0, 0));
        }
        catch (Exception e) { }
    }
}
