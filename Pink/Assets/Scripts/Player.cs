using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Platyer Movement
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
        float cameray = cameraInput.y * sensitivity * Time.deltaTime;

        xRotation -= cameray;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * cameraX);

        if (playerControls.Gameplay.Jump.triggered && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity and Jumping
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    public void OnCameraMove(InputAction.CallbackContext ctx)
    {
        cameraInput = ctx.ReadValue<Vector2>();
    }
}
