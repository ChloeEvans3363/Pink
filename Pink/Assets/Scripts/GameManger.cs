using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    private static GameManger _instance;
    
    public static GameManger Instance { get { return _instance; } }



    // Match Settings
    public float matchTime = 10.0f * 60.0f; // Total Time until a match automatically ends.
    public int matchGoal = 20; // Total Score needed to Win the Game
    public int powerUpThreshold = 5; // Amount need to activate the next Power Up
    public int maxPowerUpLimit = 2; // Total Number of Power Ups that are allowed to be active in a match

    // Match Stats
    public List<Player> players = new List<Player>(); // Total Players
    public float currentMatchTime; // Current Time of the match
    public int totalScore; // Score of all players together
    public int activePowerUps; // Total Power Ups Active
    public int pausedPlayers = 0; // Total Player that have Paused the Game

    private PowerUpManager powerUpManager = new PowerUpManager();

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        currentMatchTime = matchTime;
    }

    private void Update()
    {
        float time = Time.deltaTime;
        currentMatchTime -= time;

    }

    public void PowerUpCheck()
    {
        PowerUpManager.Instance.ActivateRandomPowerUp(players);
    }
}
