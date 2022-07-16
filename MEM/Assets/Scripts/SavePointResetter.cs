using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SavePointResetter : MonoBehaviour
{
    GameObject[] savePoints;
    void Awake()
    {
        savePoints = GameObject.FindGameObjectsWithTag("SavePoint");
        foreach (var i in savePoints)
        {
            try
            {
                i.GetComponent<SavePoint>().Reset();
            }
            catch (Exception e) { }
        }
    }
}
