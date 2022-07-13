using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointParent : MonoBehaviour
{
    [HideInInspector] public int playerReached = 0;
    SavePointController savePointController;
    [HideInInspector] public List<Collider2D> characters = new List<Collider2D>();
    //static SavePoint instance;
    [HideInInspector] public bool used = false;

    //Respawn Settings
    [Header("Respawn Settings")]
    public Vector3 respawnPosition;
    public Vector3 character1LocalPos;
    public Vector3 character2LocalPos;

    void Start()
    {
        savePointController = GameObject.Find("SavePointController").GetComponent<SavePointController>();
    }

    private void Update()
    {
        if (used) return;

        if (playerReached == 2)
        {
            savePointController.lastSavePos = respawnPosition;
            savePointController.character1LocalPos = character1LocalPos;
            savePointController.character2LocalPos = character2LocalPos;

            used = true;
        }
    }
}
