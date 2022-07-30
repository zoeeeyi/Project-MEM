using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingKiller : MonoBehaviour
{
    [Range(0, 200)]
    public float rotationSpeed;
    [HideInInspector] public float hiddenRotationSpeed;

    GameManager gameManager;

    public bool reverse = false;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        hiddenRotationSpeed = rotationSpeed * Time.deltaTime * gameManager.rotationSpeedModifier;
        hiddenRotationSpeed *= (reverse) ? -1 : 1;

        transform.Rotate(0, 0, hiddenRotationSpeed);
    }
}
