using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    SavePointController savePointController;
    List<Collider2D> characters = new List<Collider2D>();
    private Renderer rend;
    bool used = false;

    //Respawn Settings
    [Header("Respawn Settings")]
    public Vector3 respawnPosition;
    public Vector3 character1LocalPos;
    public Vector3 character2LocalPos;
    
    void Start()
    {
        rend = GetComponent<Renderer>();
        savePointController = GameObject.Find("SavePointController").GetComponent<SavePointController>();
        DontDestroyOnLoad(this);
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

            //Set color
            Color color = Color.blue;
            color.a = 0.5f;
            rend.material.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!characters.Contains(collision)) characters.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (characters.Contains(collision)) characters.Remove(collision);
        }
    }
}
