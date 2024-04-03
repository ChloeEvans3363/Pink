using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    private static GameManger _instance;
    
    public static GameManger Instance { get { return _instance; } }

    public List<Player> players = new List<Player>();

    private PowerUpManager powerUpManager = new PowerUpManager();

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
<<<<<<< HEAD
        powerUpManager.ActivatePowerUp(PowerUpManager.PowerUp.mortarRockets, players);
=======
        //powerUpManager.ActivatePowerUp(PowerUpManager.PowerUp.tripleBarrelRocketLauncher, players);
>>>>>>> 1ad36bc05a1e2903fcd4e206a71e9220c808c8a2
    }

    // Update is called once per frame
    void Update()
    {

    }
}
