using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 2f;

    [Header("Camera")]
    public float mouseSensitivity = 2f;
    public Transform cameraParent;
    public float minAngle = -60f; // Минимальный угол (вниз)
    public float maxAngle = 60f;  // Максимальный угол (вверх)
    private float cameraPitch;

    private CharacterController controller;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
    }

    void OnSprint(InputAction.CallbackContext context)
    {
        // sprintSpeed уже используется в Update
    }

    void Update()
    {
        // Вращение камеры
        /*if (cameraParent != null)
        {
            cameraParent.Rotate(Vector3.right, lookInput.y * mouseSensitivity);

            transform.Rotate(Vector3.up, lookInput.x * mouseSensitivity);
        }*/
        // Вращение камеры
        if (cameraParent != null)
        {
            // Обновляем текущий угол наклона камеры (pitch) на основе вертикального ввода
            cameraPitch -= lookInput.y * mouseSensitivity;
            // Ограничиваем угол в пределах minAngle и maxAngle
            cameraPitch = Mathf.Clamp(cameraPitch, minAngle, maxAngle);

            // Применяем поворот к родительской трансформации камеры
            // Используем Quaternion.Euler для установки угла поворота по X
            // Остальные углы (Y, Z) остаются без изменений
            Vector3 currentRotation = cameraParent.localEulerAngles;
            // Устанавливаем новый угол по оси X (pitch), Y и Z остаются как есть
            cameraParent.localEulerAngles = new Vector3(cameraPitch, currentRotation.y, currentRotation.z);

            // Поворот игрока по горизонтали (влево/вправо) по оси Y
            transform.Rotate(Vector3.up, lookInput.x * mouseSensitivity);
        }

        // Движение
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = transform.TransformDirection(move);
        move *= (inputActions.Player.Sprint.IsPressed() ? sprintSpeed : moveSpeed);

        // Гравитация
        if (!controller.isGrounded)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}