using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingKiller : MonoBehaviour
{
    [Range(0, 0.5f)]
    public float rotationSpeed;
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed);
    }
}
