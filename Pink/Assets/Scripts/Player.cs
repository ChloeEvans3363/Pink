using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    // Player Movement
    private CharacterController controller;
    private PlayerControls playerControls;
    private UnityEngine.Vector2 movementInput;
    [SerializeField] private float speed = 12f;

    // Gravity and Jumping
    private UnityEngine.Vector3 velocity;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float groundRaidus = 0.24f;
    [SerializeField] private LayerMask groundLayers;
    private float groundedOffset = 1.8f;
    private bool grounded;
    private float jumpHeight = 1.5f;
    private bool jumped = false;

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
        UnityEngine.Vector3 spherePosition = new UnityEngine.Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);

        grounded = Physics.CheckSphere(spherePosition, groundRaidus, groundLayers);

        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        // Move
        UnityEngine.Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

        controller.Move(move * speed * Time.deltaTime);

        // Camera
        float cameraX = cameraInput.x * sensitivity * Time.deltaTime;
        float cameraY = cameraInput.y * sensitivity * Time.deltaTime;

        xRotation -= cameraY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.transform.localRotation = UnityEngine.Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(UnityEngine.Vector3.up * cameraX);

        if (jumped && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity and Jumping
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // Combat
        if (reloadTimer <= 0 && shoot)
        {
            GameObject bulletObj = Instantiate(bullet, this.transform.position + camera.transform.forward * 2.0f, UnityEngine.Quaternion.identity);
            bulletObj.GetComponent<Bullet>().Shoot(camera.transform.forward);
            reloadTimer = reloadDuration;
        }
        invincibilityTimer -= Time.deltaTime;
        reloadTimer -= Time.deltaTime;
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
}
