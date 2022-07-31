using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    float smoothVelocityX;
    Vector2 startingPos;
    Vector2 targetPos;

    [HideInInspector] public bool inLevelSeletor;
    public Vector2 levelSelectorPos;
    public float cameraMoveSmoothTime;

    private void Awake()
    {
        inLevelSeletor = false;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (inLevelSeletor) targetPos = levelSelectorPos;
        else targetPos = startingPos;

        Vector2 focusPosition = targetPos;

        focusPosition.x = Mathf.SmoothDamp(transform.position.x, focusPosition.x, ref smoothVelocityX, cameraMoveSmoothTime);

        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }
}
