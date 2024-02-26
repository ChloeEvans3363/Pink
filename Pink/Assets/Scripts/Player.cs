using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    // Player Movement
    private CharacterController controller;
    private PlayerControls playerControls;
    private UnityEngine.Vector2 movementInput;
    private float accelerate = 500.0f;
    private float friction = 15;
    private float minFriction = 0.5f;
    [SerializeField] private float maxSpeed = 8f;
    private UnityEngine.Vector3 velocity;

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

    //Combat
    [SerializeField] public GameObject bullet;
    private int health = 2;
    [SerializeField] private float invincibilityDuration = 1.0f;
    private float invincibilityTimer = 0;
    [SerializeField] private float reloadDuration = 2.0f;
    private float reloadTimer = 0;
    private bool shoot = false;
    private GameObject[] spawnPoints;

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

    public UnityEngine.Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }


    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.deltaTime;
        UnityEngine.Vector3 spherePosition = new UnityEngine.Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);

        grounded = Physics.CheckSphere(spherePosition, groundRaidus, groundLayers);

        if (grounded && velocity.y < 0)
            velocity.y = -2f;

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

        // Combat
        if (reloadTimer <= 0 && shoot)
        {
            GameObject bulletObj = Instantiate(bullet, this.transform.position + camera.transform.forward * 2.0f, UnityEngine.Quaternion.identity);
            bulletObj.GetComponent<Bullet>().Shoot(camera.transform.forward);
            reloadTimer = reloadDuration;
        }
        invincibilityTimer -= Time.deltaTime;
        reloadTimer -= Time.deltaTime;

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
        }
        

        controller.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<UnityEngine.Vector2>();
    }

    public void OnCameraMove(InputAction.CallbackContext ctx)
    {
        cameraInput = ctx.ReadValue<UnityEngine.Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        jumped = ctx.action.triggered;
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        shoot = ctx.action.triggered;
    }

    public bool TakeDamage(int damage)
    {
        if(invincibilityTimer > 0) { return false; }
        this.health -= damage;
        Debug.Log("Took " + damage + " damage. Has " + this.health + " health left.");
        invincibilityTimer = invincibilityDuration;
        if(this.health <= 0) {
            Respawn();
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
        explosionVector /= distance;
        velocity += explosionVector * force * forceMulti;
    }
}
