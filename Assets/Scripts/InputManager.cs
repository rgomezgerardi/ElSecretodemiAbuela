using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameInputState
{
    Menu,
    Gameplay,
    GanaPierde,
    Pausa
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public GameInputState CurrentState { get; private set; }

    // =========================
    // EVENTOS
    // =========================

    public event Action<Vector2> OnNavigate;
    public event Action OnConfirm;
    public event Action<bool> OnLookEnemy;
    public event Action OnPause;

    private Vector2 lastMoveInput;
    private float inputDelay = 0.2f;
    private float inputTimer;

    private bool mobileLookHeld;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (CurrentState == GameInputState.Menu ||
            CurrentState == GameInputState.GanaPierde ||
            CurrentState == GameInputState.Pausa)
        {
            HandleMenuInput();
        }
        else if (CurrentState == GameInputState.Gameplay)
        {
            HandleGameplayInput();
        }
    }

    // =========================
    // STATE
    // =========================

    public void SetState(GameInputState newState)
    {
        CurrentState = newState;
        lastMoveInput = Vector2.zero;
        inputTimer = 0f;
    }

    // =========================
    // MENU INPUT
    // =========================

    private void HandleMenuInput()
    {
        Vector2 moveInput = GetMoveInput();

        if (moveInput != Vector2.zero)
        {
            if (Time.unscaledTime > inputTimer)
            {
                OnNavigate?.Invoke(moveInput);
                inputTimer = Time.unscaledTime + inputDelay;
            }
        }

        if (GetConfirmPressed())
        {
            OnConfirm?.Invoke();
        }
    }

    // =========================
    // GAMEPLAY INPUT
    // =========================

    private void HandleGameplayInput()
    {
        Vector2 moveInput = GetMoveInput();

        if (moveInput != Vector2.zero)
        {
            if (Time.time > inputTimer)
            {
                OnNavigate?.Invoke(moveInput);
                inputTimer = Time.time + inputDelay;
            }
        }

        if (GetConfirmPressed())
        {
            OnConfirm?.Invoke();
        }

        OnLookEnemy?.Invoke(GetLookHeld());

        if (GetPausePressed())
        {
            OnPause?.Invoke();
        }
    }

    // =========================
    // INPUT SOURCES
    // =========================

    private Vector2 GetMoveInput()
    {
        Vector2 input = Vector2.zero;

        // TECLADO
        if (Keyboard.current != null)
        {
            if (Keyboard.current.upArrowKey.isPressed) input.y = 1;
            if (Keyboard.current.downArrowKey.isPressed) input.y = -1;
            if (Keyboard.current.leftArrowKey.isPressed) input.x = -1;
            if (Keyboard.current.rightArrowKey.isPressed) input.x = 1;
        }

        // GAMEPAD
        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();

            if (stick.magnitude > 0.5f)
                input = stick;
        }

        return input;
    }

    private bool GetConfirmPressed()
    {
        bool pressed = false;

        // TECLADO
        if (Keyboard.current != null &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            pressed = true;
        }

        // GAMEPAD (A en Xbox, X en PlayStation)
        if (Gamepad.current != null &&
            Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            pressed = true;
        }

        return pressed;
    }

    private bool GetLookHeld()
    {
        bool held = mobileLookHeld;

        if (Keyboard.current != null &&
            Keyboard.current.wKey.isPressed)
            held = true;

        if (Gamepad.current != null &&
            Gamepad.current.rightShoulder.isPressed)
            held = true;

        return held;
    }

    private bool GetPausePressed()
    {
        bool pressed = false;

        // TECLADO (P)
        if (Keyboard.current != null &&
            Keyboard.current.pKey.wasPressedThisFrame)
        {
            pressed = true;
        }

        // GAMEPAD (Options en PS4 = startButton)
        if (Gamepad.current != null &&
            Gamepad.current.startButton.wasPressedThisFrame)
        {
            pressed = true;
        }

        return pressed;
    }

    public void TriggerLookEnemy(bool value)
    {
        mobileLookHeld = value;
    }
}