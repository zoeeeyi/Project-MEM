using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingKiller : MonoBehaviour
{
    [Range(0, 0.5f)]
    public float rotationSpeed;
    [HideInInspector]public float hiddenRotationSpeed;

    public bool reverse = false;

    void Update()
    {
        hiddenRotationSpeed = rotationSpeed;
        hiddenRotationSpeed *= (reverse) ? -1 : 1;

        transform.Rotate(0, 0, hiddenRotationSpeed);
    }
}
