using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointController : MonoBehaviour
{
    static SavePointController instance;
    public Vector2 lastSavePos;
    public GameObject characterParent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.lastSavePos = characterParent.transform.position;
            DontDestroyOnLoad(instance);
        } else
        {
            Destroy(gameObject);
        }
    }
}
