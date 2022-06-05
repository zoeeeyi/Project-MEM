using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalForce;
    public float verticalForce;
    public float maxSpeed = 5;

    Rigidbody2D playerRB;
    Rigidbody2D otherPlayerRB;
    Animator playerAnimator;
    public GameObject otherPlayer;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        //otherPlayerRB = otherPlayer.GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (otherPlayer != null)
        {
            Vector2 position = transform.position;
            if(position.x != otherPlayer.transform.position.x)
            {
                //playerRB.velocity = new Vector2(0,playerRB.velocity.y);
                position.x = otherPlayer.transform.position.x;
                playerRB.MovePosition(position);
            }
        }
    }

    void FixedUpdate()
    {
        playerAnimator.SetFloat("Speed", Mathf.Abs(playerRB.velocity.x));

        if (Mathf.Abs(playerRB.velocity.x) < maxSpeed)
        {
            if (Input.GetKey(KeyCode.D))
            {
                playerRB.AddForce(horizontalForce * Vector2.right);
                //Movement
                playerAnimator.SetFloat("MoveX", Vector2.right.x);
                //Animation
            }
            if (Input.GetKey(KeyCode.A))
            {
                playerRB.AddForce(horizontalForce * Vector2.left);
                playerAnimator.SetFloat("MoveX", Vector2.left.x);
            }
        }
        else
        {
            playerRB.AddForce(Vector2.zero);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRB.AddForce(verticalForce * Vector2.up, ForceMode2D.Impulse);
        }
    }
}