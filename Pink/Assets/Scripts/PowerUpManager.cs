using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager _instance;

    public static PowerUpManager Instance { get { return _instance; } }

    public enum PowerUp
    {
        speed,
        explosionLanding,
        tripleBarrelRocketLauncher,
        mortarRockets,
        moonGravity,
        bigExplosion
    }

    private PowerUp previousPowerUp = (PowerUp)(-1); // The Previous Power Up Activated

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
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
                foreach (Player player in players)
                    player.ExplosionLanding = true;
                break;

            case PowerUp.tripleBarrelRocketLauncher:
                foreach (Player player in players)
                    player.shootAction += TripleAttack;
                break;

            case PowerUp.mortarRockets:
                foreach (Player player in players)
                    player.GetComponent<Player>().IsLobShot = true;
                break;

            case PowerUp.moonGravity:
                foreach (Player player in players)
                {
                    player.Jump = 15;
                    player.Gravity = 10;
                }
                break;

            case PowerUp.bigExplosion:
                foreach (Player player in players)
                    player.OuterRadius = 20;
                break;

        }
    }

    private void TripleAttack(Player player)
    {
        GameObject bulletObjRight = Instantiate(player.bullet, player.transform.position + player.PlayerCamera.transform.forward * 2.0f, Quaternion.Euler(new Vector3(0, 22.5f, 0)));
        bulletObjRight.GetComponent<Bullet>().Shoot(player.PlayerCamera.transform.forward, player.gameObject);
        bulletObjRight.GetComponent<Bullet>().OuterRadius = player.GetComponent<Player>().OuterRadius;
        bulletObjRight.GetComponent<Bullet>().Explosion.GetComponent<Explosion>().explosionForce = 1;
        bulletObjRight.GetComponent<Bullet>().isLobShot = player.GetComponent<Player>().IsLobShot;
        bulletObjRight.GetComponent<Rigidbody>().freezeRotation = true;

        GameObject bulletObjLeft = Instantiate(player.bullet, player.transform.position + player.PlayerCamera.transform.forward * 2.0f, Quaternion.Euler(new Vector3(0, -22.5f, 0)));
        bulletObjLeft.GetComponent<Bullet>().Shoot(player.PlayerCamera.transform.forward, player.gameObject);
        bulletObjLeft.GetComponent<Bullet>().OuterRadius = player.GetComponent<Player>().OuterRadius;
        bulletObjLeft.GetComponent<Bullet>().Explosion.GetComponent<Explosion>().explosionForce = 1;
        bulletObjLeft.GetComponent<Bullet>().isLobShot = player.GetComponent<Player>().IsLobShot;
        bulletObjLeft.GetComponent<Rigidbody>().freezeRotation = true;
    }

    private PowerUp GetRandomPowerUp()
    {
        Array powerUps = Enum.GetValues(typeof(PowerUp));
        return (PowerUp)powerUps.GetValue(UnityEngine.Random.Range(0, powerUps.Length));
    }

    public void ActivateRandomPowerUp(List<Player> players)
    {
        PowerUp randomPowerUp = GetRandomPowerUp();

        Debug.Log(randomPowerUp);

        // Ensure the newly selected power-up is different from the previous one
        while (randomPowerUp == previousPowerUp)
        {
            randomPowerUp = GetRandomPowerUp();
        }

        // Activate the randomly selected power-up
        ActivatePowerUp(randomPowerUp, players);
        previousPowerUp = randomPowerUp;
    }

}
