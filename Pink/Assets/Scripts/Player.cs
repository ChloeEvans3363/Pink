using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Player Movement
    private CharacterController controller;
    private PlayerControls playerControls;
    private Vector2 movementInput;
    [SerializeField] private float speed = 12f;

    // Gravity and Jumping
    private Vector3 velocity;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float groundRaidus = 0.24f;
    [SerializeField] private LayerMask groundLayers;
    private float groundedOffset = 1.8f;
    private bool grounded;
    private float jumpHeight = 1.5f;

    // Camera Movement
    [SerializeField] private Camera camera;
    [SerializeField] private float sensitivity = 30f;
    float xRotation = 0f;
    private Vector2 cameraInput;

    //Combat
    [SerializeField] public GameObject bullet;
    [SerializeField] public int health = 2;
    [SerializeField] private float invincibilityDuration = 1.0f;
    private float invincibilityTimer = 0;
    [SerializeField] private float reloadDuration = 2.0f;
    private float reloadTimer = 0;

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
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);

        grounded = Physics.CheckSphere(spherePosition, groundRaidus, groundLayers);

        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        // Move
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

        controller.Move(move * speed * Time.deltaTime);

        // Camera
        float cameraX = cameraInput.x * sensitivity * Time.deltaTime;
        float cameraY = cameraInput.y * sensitivity * Time.deltaTime;

        xRotation -= cameraY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * cameraX);

        if (playerControls.Gameplay.Jump.triggered && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity and Jumping
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // Combat
        if (reloadTimer <= 0 && Input.GetMouseButtonDown(0))
        {
            GameObject bulletObj = Instantiate(bullet, this.transform.position + camera.transform.forward * 2.0f, Quaternion.identity);
            bulletObj.GetComponent<Bullet>().Shoot(camera.transform.forward);
            reloadTimer = reloadDuration;
        }
        invincibilityTimer -= Time.deltaTime;
        reloadTimer -= Time.deltaTime;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    public void OnCameraMove(InputAction.CallbackContext ctx)
    {
        cameraInput = ctx.ReadValue<Vector2>();
    }
    
    public void TakeDamage(int damage)
    {
        if(invincibilityTimer > 0) { return; }
        Debug.Log("Took " + damage + " damage");
        health -= damage;
        invincibilityTimer = invincibilityDuration;
        if(health <= 0) { Die(); }
    }

    private void Die()
    {

    }
}
