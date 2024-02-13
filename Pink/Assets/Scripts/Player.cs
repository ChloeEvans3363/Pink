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

    // Camera Movement
    public Camera camera;
    public float sensitivity = 30f;
    float xRotation = 0f;
    public float speed = 12f;
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
