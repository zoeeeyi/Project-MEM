using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointController : MonoBehaviour
{
    static SavePointController instance;
    public Vector3 lastSavePos;

    [HideInInspector] public Vector3 character1LocalPos;
    [HideInInspector] public Vector3 character2LocalPos;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            //initialize the last save position
            GameObject characterParent = GameObject.Find("Character Parent");
            GameObject character1 = GameObject.Find("Character");
            GameObject character2 = GameObject.Find("CharacterFlipped");
            instance.lastSavePos = characterParent.transform.position;
            instance.character1LocalPos = character1.transform.position - lastSavePos;
            instance.character2LocalPos = character2.transform.position - lastSavePos;

            DontDestroyOnLoad(instance);
        } else
        {
            Destroy(gameObject);
        }
    }
}
