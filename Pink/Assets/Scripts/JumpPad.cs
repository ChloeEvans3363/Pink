using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jump = 40;
    private float playerJump;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") { return; }

        other.gameObject.GetComponent<Player>().Grounded = false;
        other.gameObject.GetComponent<Player>().Jumped = true;
        playerJump = other.gameObject.GetComponent<Player>().Jump;
        other.gameObject.GetComponent<Player>().Jump = jump;
        Debug.Log("jump pad");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") { return; }
        other.gameObject.GetComponent<Player>().Jumped = false;
        other.gameObject.GetComponent<Player>().Jump = playerJump;
    }
}
