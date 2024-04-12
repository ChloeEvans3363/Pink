using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jump = 60;
    private float playerJump;
    private AudioSource boing;

    private void Start()
    {
        boing = GetComponent<AudioSource>();   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") { return; }

        playerJump = other.gameObject.GetComponent<Player>().Jump;
        other.gameObject.GetComponent<Player>().Jump = jump;

        other.gameObject.GetComponent<Player>().Jumped = true;
        other.gameObject.GetComponent<Player>().Grounded = true;

        boing.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") { return; }
        other.gameObject.GetComponent<Player>().Jumped = false;
        other.gameObject.GetComponent<Player>().Jump = playerJump;
    }
}
