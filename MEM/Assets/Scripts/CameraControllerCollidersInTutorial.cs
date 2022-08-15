using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerCollidersInTutorial : MonoBehaviour
{
    CameraController camera;
    public float speed;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
