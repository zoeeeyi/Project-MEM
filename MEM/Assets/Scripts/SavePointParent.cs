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

    //Floating Settings
    [Header("Floating Settings")]
    public float floatingRange;
    public float floatingSmoothTime;
    public float floatingScaleModifier;

    void Start()
    {
        savePointController = GameObject.FindGameObjectWithTag("SavePointController").GetComponent<SavePointController>();
    }

    private void Update()
    {
        if (used) return;

        if (characters.Count == 2)
        {
            savePointController.lastSavePos = respawnPosition;
            savePointController.character1LocalPos = character1LocalPos;
            savePointController.character2LocalPos = character2LocalPos;

            used = true;
        }
    }
}
