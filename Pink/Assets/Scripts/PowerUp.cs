using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        speed,
        explosionLanding,
        tripleBarrelRocketLauncher,
        mortarRockets,
        moonGravity
    }

    private List<Player> players;

    void Start()
    {
        players = GameManger.Instance.players;
    }

    public void ActivatePowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.speed:

                break;

            case PowerUpType.explosionLanding:
                break;

            case PowerUpType.tripleBarrelRocketLauncher:
                break;

            case PowerUpType.mortarRockets:
                break;

            case PowerUpType.moonGravity:
                break;

        }
    }
}
