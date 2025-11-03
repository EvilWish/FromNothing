using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput input;                 
    private PlayerInput.OnFootActions onFoot;

    public bool isUIOpen;
    public bool jumpEnabled = false;

    private PlayerMotor motor;
    private PlayerLook look;

    // cached inputs
    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        Instance = this;
        input = new PlayerInput();
        onFoot = input.OnFoot;
    }

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        onFoot.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        onFoot.Movement.canceled += ctx => moveInput = Vector2.zero;

        onFoot.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        onFoot.Look.canceled += ctx => lookInput = Vector2.zero;

        if(jumpEnabled)
            onFoot.Jump.performed += _ => motor.Jump();

        // Debug 
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        onFoot.Enable();
        input.UI.Disable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
        input.UI.Disable();
    }

    private void FixedUpdate()
    {
        if (!isUIOpen)
            motor.ProcessMove(moveInput);
    }

    private void LateUpdate()
    {
        if (!isUIOpen)
            look.ProcessLook(lookInput);
    }

    public void SetGameplayEnabled(bool enabled)
    {
        isUIOpen = !enabled;

        if (enabled)
        {
            onFoot.Enable();
            input.UI.Disable();
        }
        else
        {
            onFoot.Disable();
            input.UI.Enable();

            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
        }
    }
}
