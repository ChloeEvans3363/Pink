using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Player Movement
    private CharacterController controller;
    private InputActionMap gameplay;
    private InputActionAsset inputAsset;
    private InputAction movement;
    private InputAction cameraMove;
    private UnityEngine.Vector2 movementInput;
    private float accelerate = 500.0f;
    private float friction = 15;
    private float minFriction = 0.5f;
    [SerializeField] private float maxSpeed = 8f;
    private UnityEngine.Vector3 velocity;
    private bool land = false;

    // Gravity and Jumping
    [SerializeField] private LayerMask groundLayers;
    private float groundRaidus = 0.24f;
    private float groundedOffset = 1.8f;
    private bool grounded;
    private bool jumped = false;
    private float airControl = 16f;
    private float airForwardAcceleration = 8f;
    private float airAccelCoeff = 1f;
    private float airDecelCoeff = 1.5f;
    [SerializeField] private float gravity = 24f;
    [SerializeField] private float jump = 8f;

    // Camera Movement
    [SerializeField] private Camera camera;
    [SerializeField] private float sensitivity = 30f;
    float xRotation = 0f;
    private UnityEngine.Vector2 cameraInput;

    // Combat
    [SerializeField] public GameObject bullet;
    [SerializeField] public GameObject explosionObject;
    [SerializeField] public GameObject bulletSpawn;
    private int health = 1;
    [SerializeField] private float invincibilityDuration = 1.0f;
    private float invincibilityTimer = 0;
    [SerializeField] private float reloadDuration = 2.0f;
    private float reloadTimer = 0;
    private bool shoot = false;
    private GameObject[] spawnPoints;
    private float outerRadius = 10f;

    // Scoring
    public int score = 0;
    [SerializeField] public TMP_Text scoreText;

    // Winning & Respawn
    [SerializeField] private Transform gameCanvas;
    [SerializeField] private GameObject respawnScreen;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private float respawnTime = 3f;
    private float currentRespawnTime = 0f;
    private bool isRespawning = false;
    private int winningPlayerIndex;
    [SerializeField] private TMP_Text gameTimer;

    // Pausing
    private bool pause = false;
    private bool gamePaused = false;
    private bool prevPauseState = false;
    [SerializeField] private GameObject pauseScreen;

    // Powerups
    private bool explosionLanding = false;
    private bool isLobShot = false;

    // Get Sets
    public bool Grounded
    {
        get { return grounded; }
        set { grounded = value; }
    }

    public bool Jumped
    {
        get { return jumped; }
        set { jumped = value; }
    }

    public float Jump
    {
        get { return jump; }
        set { jump = value; }
    }

    public float Speed
    {
        get { return maxSpeed; }
        set { maxSpeed = value; }
    }

    public UnityEngine.Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public float Gravity
    {
        get { return gravity; }
        set { gravity = value; }
    }

    public Camera PlayerCamera
    {
        get { return camera; }
    }

    public float ReloadTimer
    {
        get { return reloadTimer; }
        set { reloadTimer = value; }
    }

    public float ReloadDuration
    {
        get { return reloadDuration; }
        set { reloadDuration = value; }
    }

    public float OuterRadius
    {
        get { return outerRadius; }
        set { outerRadius = value; }
    }

    public bool ExplosionLanding
    {
        set { explosionLanding = value; }
    }

    public bool IsLobShot
    {
        get { return isLobShot; }
        set { isLobShot = value; }
    }

    private void Awake()
    {
        inputAsset = this.GetComponent<PlayerInput>().actions;
        gameplay = inputAsset.FindActionMap("Gameplay");
        //playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        movement = gameplay.FindAction("Move");
        movement.Enable();

        cameraMove = gameplay.FindAction("CameraMove");
        cameraMove.Enable();

        gameplay.FindAction("Jump").performed += OnJump;
        gameplay.FindAction("Jump").canceled += OnJump;
        gameplay.FindAction("Jump").Enable();

        shootAction = Attack;

        gameplay.FindAction("Shoot").performed += OnShoot;
        gameplay.FindAction("Shoot").canceled += OnShoot;
        gameplay.FindAction("Shoot").Enable();

        gameplay.FindAction("Pause").performed += OnPause;
        gameplay.FindAction("Pause").canceled += OnPause;
        gameplay.FindAction("Pause").Enable();

        gameplay.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
        cameraMove.Disable();

        gameplay.FindAction("Jump").performed -= OnJump;
        gameplay.FindAction("Jump").canceled -= OnJump;
        gameplay.FindAction("Jump").Disable();

        gameplay.FindAction("Shoot").performed -= OnShoot;
        gameplay.FindAction("Shoot").canceled -= OnShoot;
        gameplay.FindAction("Shoot").Disable();

        gameplay.FindAction("Pause").performed -= OnPause;
        gameplay.FindAction("Pause").canceled -= OnPause;
        gameplay.FindAction("Pause").Disable();

        gameplay.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        GameManger.Instance.players.Add(this);

        // Set Up Score Display
        scoreText.text = score.ToString() + " / " + GameManger.Instance.matchGoal.ToString();
    }

    // Update is called once per frame
    void Update()
    {


        // Combat
        if (reloadTimer <= 0 && shoot && !isRespawning)
        {
            shootAction(this);
        }
        invincibilityTimer -= Time.deltaTime;
        reloadTimer -= Time.deltaTime;

        // End Game
        if(GameManger.Instance.currentMatchTime <= 0 || score >= GameManger.Instance.matchGoal)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < players.Length; i++)
            {
                GameObject player = players[i];
                if (!player.activeInHierarchy) continue;
                if (player.GetComponent<Player>().score >= GameManger.Instance.matchGoal)
                {
                    winningPlayerIndex = i;
                    break;
                }
            }

            winText.text = "PLAYER " + winningPlayerIndex + " WINS!";
            winScreen.SetActive(true);

            // Load Main Menu after Game Ends
            StartCoroutine(LoadMenuAfterDelay());
        }

        Movement();

        if (isRespawning)
        {
            currentRespawnTime -= Time.deltaTime;
            int seconds = Mathf.CeilToInt(currentRespawnTime);
            countdownText.SetText(seconds.ToString());

            // Hide Body
            this.transform.GetChild(0).gameObject.SetActive(false);

            if (currentRespawnTime <= 0f)
            {
                Respawn();
                this.transform.GetChild(0).gameObject.SetActive(true);
                respawnScreen.SetActive(false);
                isRespawning = false;
                countdownText.SetText("");
            }
        }

        controller.Move(velocity * Time.deltaTime);

        if (pause && !prevPauseState)
        {
            if (gamePaused == false)
            {
                // Pause Game
                gamePaused = true;
                pauseScreen.SetActive(true);
                Time.timeScale = 0f;
                GameManger.Instance.pausedPlayers += 1;

                // Find all Buttons in menu
                Button[] buttons = pauseScreen.GetComponentsInChildren<Button>();

                if (buttons.Length > 0)
                {
                    // Set focus to first button found
                    EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);

                }
            }
            else
            {
                pauseScreen.SetActive(false);
                gamePaused = false;

                // Checks how many players are paused
                GameManger.Instance.pausedPlayers -= 1;
                Debug.Log(GameManger.Instance.pausedPlayers);

                if (GameManger.Instance.pausedPlayers == 0)
                {
                    // UnPaused Game
                    Time.timeScale = 1f;
                }

            }
        }

        // Update the previous pause state
        prevPauseState = pause;

        // Set Match Display Timer to elapsed time in game

        gameTimer.text = GameManger.Instance.currentMatchTime.ToString("N0");
    }

    private void Movement()
    {
        if (isRespawning)
        {
            velocity = UnityEngine.Vector3.zero;
            return;
        }

        // Setting up inputs
        movementInput = movement.ReadValue<UnityEngine.Vector2>();
        cameraInput = cameraMove.ReadValue<UnityEngine.Vector2>();

        float time = Time.deltaTime;
        UnityEngine.Vector3 spherePosition = new UnityEngine.Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);

        grounded = Physics.CheckSphere(spherePosition, groundRaidus, groundLayers);

        if (grounded && land && explosionLanding)
        {
            land = false;
            GameObject explosionObj = Instantiate(explosionObject, this.transform.position, UnityEngine.Quaternion.identity);
            explosionObj.transform.localScale = new UnityEngine.Vector3(outerRadius, outerRadius, outerRadius);
            explosionObj.GetComponent<Explosion>().outerRadius = outerRadius;
            explosionObj.GetComponent<Explosion>().owner = this.gameObject;
            explosionObj.GetComponent<Explosion>().explosionForce = 1;
        }

        // Wishdir
        UnityEngine.Vector3 move = new UnityEngine.Vector3(movementInput.x, 0, movementInput.y).normalized;
        UnityEngine.Vector3 wishdir = UnityEngine.Vector3.ProjectOnPlane(transform.TransformDirection(move), UnityEngine.Vector3.down).normalized;

        // Camera
        float cameraX = cameraInput.x * sensitivity * Time.deltaTime;
        float cameraY = cameraInput.y * sensitivity * Time.deltaTime;

        xRotation -= cameraY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.transform.localRotation = UnityEngine.Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(UnityEngine.Vector3.up * cameraX);

        // Movement
        if (grounded)
        {
            ApplyFriction(time);
            ApplyAcceleration(wishdir, accelerate, time);

            velocity = UnityEngine.Vector3.ProjectOnPlane(velocity, UnityEngine.Vector3.down);
            if (jumped)
            {
                velocity += -UnityEngine.Vector3.down * jump;
            }
        }
        else
        {
            float coeff = UnityEngine.Vector3.Dot(velocity, wishdir) > 0 ? airAccelCoeff : airDecelCoeff;

            ApplyAcceleration(wishdir, coeff, time);

            if (Mathf.Abs(move.z) > 0.0001)
            {
                ApplyAirControl(move, wishdir, time);
            }


            velocity += UnityEngine.Vector3.down * (gravity * time);
            land = true;
        }
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        jumped = ctx.action.triggered;
    }

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        shoot = ctx.action.triggered;
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        pause = ctx.action.triggered;
    }

    public delegate void delegateShoot(Player player);
    public delegateShoot shootAction;

    private void Attack(Player player)
    {
        GameObject bulletObj = Instantiate(bullet, bulletSpawn.transform.position + camera.transform.forward * 2.0f, UnityEngine.Quaternion.identity);
        bulletObj.GetComponent<Bullet>().OuterRadius = outerRadius;
        bulletObj.GetComponent<Bullet>().Shoot(camera.transform.forward, this.gameObject);
        bulletObj.GetComponent<Bullet>().isLobShot = isLobShot;
        bulletObj.GetComponent<Rigidbody>().freezeRotation = true;
        reloadTimer = reloadDuration;
    }

    public bool TakeDamage(int damage, GameObject explosion)
    {
        // If shooting self
        if (explosion.GetComponent<Explosion>().owner.Equals(this.gameObject))
        {
            AddExplosionForce(explosion.transform.position,
                explosion.GetComponent<Explosion>().outerRadius,
                explosion.GetComponent<Explosion>().explosionForce);

            return false;
        }

        // If player has iFrames, do nothing
        if (invincibilityTimer > 0) { return false; }

        // If another player has shot them, take damage
        this.health -= damage;
        Debug.Log("Took " + damage + " damage. Has " + this.health + " health left.");
        invincibilityTimer = invincibilityDuration;
        if(this.health <= 0) {
            explosion.GetComponent<Explosion>().owner.GetComponent<Player>().AddScore();
            respawnScreen.SetActive(true);
            currentRespawnTime = respawnTime;
            isRespawning = true;
            return true;
        }
        return false;
    }

    private void Respawn()
    {
        gameObject.SetActive(false);
        this.health = 2;
        UnityEngine.Vector3 newPos = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        Debug.Log("Player has died, respawning at "+newPos);
        transform.position = newPos;
        gameObject.SetActive(true);
    }

    // Credit to PhobicGunner and Atil for this code
    private void ApplyAcceleration(UnityEngine.Vector3 wishDir, float acceleration, float time)
    {
        float y = velocity.y;

        float addSpeed, accelSpeed, currentSpeed;

        UnityEngine.Vector3 flatVel = velocity;
        flatVel.y = 0f;

        currentSpeed = UnityEngine.Vector3.Dot(flatVel, wishDir);

        addSpeed = maxSpeed - currentSpeed;

        if (addSpeed <= 0f)
            return;

        accelSpeed = acceleration * maxSpeed * time;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        velocity += accelSpeed * wishDir;

        velocity.y = y;
    }

    // Credit to PhobicGunner and Atil for this code

    private void ApplyFriction(float time)
    {
        float y = velocity.y;

        velocity.y = 0f;

        float currentSpeed = velocity.magnitude;
        if (currentSpeed <= 0.00001f)
        {
            return;
        }

        float downLimit = Mathf.Max(currentSpeed, minFriction);
        float dropAmount = currentSpeed - (downLimit * friction * time);
        if (dropAmount < 0)
        {
            dropAmount = 0;
        }

        dropAmount /= currentSpeed;

        if (float.IsNaN(dropAmount))
            velocity = UnityEngine.Vector3.up * y;
        else
        {
            velocity *= dropAmount;

            velocity.y = y;
        }
    }

    // Credit to Atil for this code
    private void ApplyAirControl(UnityEngine.Vector3 input, UnityEngine.Vector3 accelDir, float time)
    {
        UnityEngine.Vector3 playerHorizontalDirection = UnityEngine.Vector3.ProjectOnPlane(velocity, UnityEngine.Vector3.down).normalized;
        float playerHorizontalSpeed = UnityEngine.Vector3.ProjectOnPlane(velocity, UnityEngine.Vector3.down).magnitude;

        float dot = UnityEngine.Vector3.Dot(playerHorizontalDirection, accelDir);
        if (dot > 0)
        {
            float k = airControl * dot * dot * time;

            bool isPureForward = Mathf.Abs(input.x) < 0.0001f && Mathf.Abs(input.z) > 0;
            if (isPureForward)
            {
                k *= airForwardAcceleration;
            }

            // A little bit closer to accelDir
            playerHorizontalDirection = playerHorizontalDirection * playerHorizontalSpeed + accelDir * k;
            playerHorizontalDirection.Normalize();

            // Assign new direction, without touching the vertical speed
            velocity = UnityEngine.Vector3.ProjectOnPlane(playerHorizontalDirection * playerHorizontalSpeed, UnityEngine.Vector3.down) 
                + UnityEngine.Vector3.up * (UnityEngine.Vector3.Dot(velocity, UnityEngine.Vector3.up));
        }

    }

    // Credit to PhobicGunner for this
    public void AddExplosionForce(UnityEngine.Vector3 position, float radius, float force)
    {
        UnityEngine.Vector3 explosionVector = transform.position - position;
        float distance = explosionVector.magnitude;

        if (distance > radius)
            return;
        float forceMulti = 1f - (distance/radius);

        if(distance != 0)
            explosionVector /= distance;

        velocity += explosionVector * force * forceMulti;
    }

    private IEnumerator LoadMenuAfterDelay()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);
        // Load the Menu scene
        SceneManager.LoadScene("Menu");
    }

    public void ResumeMatch()
    {
        gamePaused = false;
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ExitMatch()
    {
        SceneManager.LoadScene("Menu");
    }

    public void AddScore()
    {
        score++;
        GameManger.Instance.totalScore++;
        GameManger.Instance.PowerUpCheck();
        scoreText.text = score.ToString() + " / " + GameManger.Instance.matchGoal.ToString();
    }
}
