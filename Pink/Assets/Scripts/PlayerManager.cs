using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager playerInputManger;
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    private GameObject[] playerList;

    public Canvas playerUI;


    private void OnEnable() 
    {
        playerInputManger = GetComponent<PlayerInputManager>();
        playerList = new GameObject[] {player1, player2, player3, player4};
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInputManger.playerPrefab = playerList[playerInputManger.playerCount];

    }
}
