using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager _instance;

    public static PowerUpManager Instance { get { return _instance; } }

    [SerializeField] private TMP_Text powerUpText;
    [SerializeField] private float activePowerUpDisplayDelay = 3f; // How Long it shows what power up was just activated

    private Coroutine displayCoroutine; // Coroutine reference to control text display duration


    public enum PowerUp
    {
        speed,
        explosionLanding,
        tripleBarrelRocketLauncher,
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
        string powerUpDisplayed = "None"; // What is shown to the player

        switch (powerUp)
        {
            case PowerUp.speed:
                powerUpDisplayed = "Speed Boost";
                foreach (Player player in players)
                    player.Speed += 20;
                break;

            case PowerUp.explosionLanding:
                powerUpDisplayed = "Explosive Landing";
                foreach (Player player in players)
                    player.ExplosionLanding = true;
                break;

            case PowerUp.tripleBarrelRocketLauncher:
                powerUpDisplayed = "Triple Barrel";
                foreach (Player player in players)
                    player.shootAction += TripleAttack;
                break;

            case PowerUp.moonGravity:
                powerUpDisplayed = "Moon Gravity";
                foreach (Player player in players)
                {
                    player.Jump = 15;
                    player.Gravity = 18;
                }
                break;

            case PowerUp.bigExplosion:
                powerUpDisplayed = "Big Explosion";
                foreach (Player player in players)
                    player.OuterRadius = 20;
                break;

        }
        // Set power up text
        powerUpText.text = powerUpDisplayed + " Power Up Activated!";

        // Start coroutine to clear the text after a delay
        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);
        displayCoroutine = StartCoroutine(ClearPowerUpText());
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

    private IEnumerator ClearPowerUpText()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(activePowerUpDisplayDelay);

        // Clear the power up text
        powerUpText.text = "";
    }

}
