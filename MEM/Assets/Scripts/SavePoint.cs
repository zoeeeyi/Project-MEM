using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    SavePointController savePointController;
    List<Collider2D> characters = new List<Collider2D>();
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        savePointController = GameObject.Find("SavePointController").GetComponent<SavePointController>();
    }

    private void Update()
    {
        if (characters.Count == 2)
        {
            savePointController.lastSavePos = transform.position;
            rend.material.color = Color.blue;
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
