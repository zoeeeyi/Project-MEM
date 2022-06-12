using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBound : MonoBehaviour
{
    public GameManager gameManager;
    private void Start()
    {
        gameManager = GetComponent<GameManager>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        gameManager.gameOver = true;
    }
}
