using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointController : MonoBehaviour
{
    //static SavePointController instance;
    [HideInInspector] public Vector3 lastSavePos;

    [HideInInspector] public Vector3 character1LocalPos;
    [HideInInspector] public Vector3 character2LocalPos;

    Vector3 lastSavePos_backup;
    Vector3 character1LocalPos_backup;
    Vector3 character2LocalPos_backup;


    private void Awake()
    {
        //initialize the last save position
        GameObject characterParent = GameObject.Find("Character Parent");
        GameObject character1 = GameObject.Find("Character");
        GameObject character2 = GameObject.Find("CharacterFlipped");

        //Read the positions before enabling scripts
        lastSavePos = characterParent.transform.position;
        character1LocalPos = character1.transform.position - lastSavePos;
        character2LocalPos = character2.transform.position - lastSavePos;

        characterParent.GetComponent<PlayerInputParent>().enabled = true;
        character1.GetComponent<PlayerInput>().enabled = true;
        character2.GetComponent<PlayerInput>().enabled = true;
    }

    public void SetSavePosition(Vector3 _lastSavePos, Vector3 _character1LocalPos, Vector3 _character2LocalPos)
    {
        //Backup positions for revert purposes
        lastSavePos_backup = lastSavePos;
        character1LocalPos_backup = character1LocalPos;
        character2LocalPos_backup = character2LocalPos;

        //Set new positions
        lastSavePos = _lastSavePos;
        character1LocalPos = _character1LocalPos;
        character2LocalPos = _character2LocalPos;
    }

    public void RevertSavePosition()
    {
        lastSavePos = lastSavePos_backup;
        character1LocalPos = character1LocalPos_backup;
        character2LocalPos = character2LocalPos_backup;
    }
}
