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
        Debug.Log(other.gameObject.GetComponent<Player>().Jumped);
        other.gameObject.GetComponent<Player>().Grounded = false;
        playerJump = other.gameObject.GetComponent<Player>().Jump;
        other.gameObject.GetComponent<Player>().Jump = jump;

        if (other.gameObject.GetComponent<Player>().Jumped)
            other.gameObject.GetComponent<Player>().Jump += 15;

            other.gameObject.GetComponent<Player>().Jumped = true;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") { return; }
        other.gameObject.GetComponent<Player>().Jumped = false;
        other.gameObject.GetComponent<Player>().Jump = playerJump;
    }
}
