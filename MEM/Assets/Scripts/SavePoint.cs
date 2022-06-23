using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    SavePointController savePointController;
    // Start is called before the first frame update
    void Start()
    {
        savePointController = GameObject.Find("SavePointController").GetComponent<SavePointController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            savePointController.lastSavePos = transform.position;
        }
    }
}
