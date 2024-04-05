using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    private static GameManger _instance;
    
    public static GameManger Instance { get { return _instance; } }

    public List<Player> players = new List<Player>();

    public int pausedPlayers = 0;

    // Match Settings
    public float matchTime = 10.0f * 60.0f; // Total Time until a match automatically ends.
    public int matchGoal = 20; // Total Score needed to Win the Game
    public int powerUpThreshold = 5; // Amount need to activate the next Power Up
    public int maxPowerUpLimit = 2; // Total Number of Power Ups that are allowed to be active in a match

    // Match Stats
    public float currentMatchTime; // Current Time of the match
    public int totalScore; // Score of all players together
    public int activePowerUps; // Total Power Ups Active

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
        if (totalScore % powerUpThreshold == 0 && activePowerUps < powerUpThreshold)
        {
            PowerUpManager.Instance.ActivateRandomPowerUp(players);
            activePowerUps++;
        }
    }
}
