using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointController : MonoBehaviour
{
    static SavePointController instance;
    public Vector2 lastSavePos;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.lastSavePos = transform.position;
            DontDestroyOnLoad(instance);
        } else
        {
            Destroy(gameObject);
        }
    }
}
