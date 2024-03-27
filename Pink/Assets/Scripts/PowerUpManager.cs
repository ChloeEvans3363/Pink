using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PowerUpManager : MonoBehaviour
{
    public enum PowerUp
    {
        speed,
        explosionLanding,
        tripleBarrelRocketLauncher,
        mortarRockets,
        moonGravity
    }

    public void ActivatePowerUp(PowerUp powerUp, List<Player> players)
    {
        switch (powerUp)
        {
            case PowerUp.speed:
                foreach (Player player in players)
                    player.Speed += 20;
                break;

            case PowerUp.explosionLanding:
                break;

            case PowerUp.tripleBarrelRocketLauncher:
                break;

            case PowerUp.mortarRockets:
                break;

            case PowerUp.moonGravity:
                foreach (Player player in players)
                {
                    player.Jump = 20;
                    player.Gravity = 10;
                }
                break;

        }
    }
}
