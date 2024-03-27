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

    private GameObject bullet;
    private float reloadTimer;
    private float reloadDuration;

    public GameObject Bullet
    {
        set { bullet = value; }
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
                {

                }
                break;

            case PowerUp.tripleBarrelRocketLauncher:
                foreach (Player player in players)
                    player.shootAction = TripleAttack;
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

    private void TripleAttack(Player player)
    {
        GameObject bulletObj = Instantiate(bullet, player.transform.position + player.PlayerCamera.transform.forward * 2.0f, UnityEngine.Quaternion.identity);
        GameObject bulletObjLeft = Instantiate(bullet, player.transform.position + player.PlayerCamera.transform.forward * 2.0f, UnityEngine.Quaternion.identity);
        bulletObj.GetComponent<Bullet>().Shoot(player.PlayerCamera.transform.forward, player.gameObject);
        bulletObjLeft.GetComponent<Bullet>().Shoot(player.PlayerCamera.transform.forward, player.gameObject);
        player.ReloadTimer = player.ReloadDuration;
    }
}
