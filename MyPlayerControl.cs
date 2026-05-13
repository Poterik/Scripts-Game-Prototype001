using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MyPlayerControl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    //public float sprintSpeed = 10f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 10f;

    [Header("Camera")]
    public Transform playerCamera;
    public Transform cameraOrbit;
    public float cameraDistance = 3f;
    public float cameraHeight = 1.5f;
    public float mouseSensivity = 0.5f;
    public float minAngle = -30f;
    public float maxAngle = 60f;
    private float cameraPitch;
    private float cameraNoX;

    [Header("References")]
    private CharacterController controller;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;

    public bool isMenu = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();
        playerCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void LateUpdate()
    {
        if (isMenu) return;

        HandleOrbit();
    }

    private void HandleOrbit()
    {
        if (cameraOrbit != null)
        {
            cameraOrbit.position = transform.position;

            cameraOrbit.Rotate(Vector3.up, lookInput.x * mouseSensivity);

            cameraPitch -= lookInput.y * mouseSensivity;
            cameraPitch = Mathf.Clamp(cameraPitch, minAngle, maxAngle);
            playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0, 0);
        }
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = new Vector3(moveInput.x, 0, moveInput.y);

        if (worldDirection != Vector3.zero && cameraOrbit != null)
        {
            worldDirection = cameraOrbit.TransformDirection(worldDirection);
            worldDirection.y = 0;
            worldDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(worldDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (!controller.isGrounded)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime * 2f;
        }

        //float speed = inputActions.Player.Sprint.IsPressed() ? sprintSpeed : moveSpeed;
        Vector3 moveVelocity = worldDirection * moveSpeed;

        moveVelocity.y = verticalVelocity;
        controller.Move(moveVelocity * Time.deltaTime);
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}
