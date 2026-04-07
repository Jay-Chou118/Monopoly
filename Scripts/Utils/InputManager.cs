using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerInput playerInput;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        playerInput = GetComponent<PlayerInput>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public bool IsMouseButtonPressed(int button)
    {
        return Mouse.current != null && Mouse.current.GetButton(button).isPressed;
    }

    public bool IsKeyPressed(Key key)
    {
        return Keyboard.current != null && Keyboard.current[key].isPressed;
    }

    public Vector2 GetMousePosition()
    {
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
    }

    public Vector2 GetMouseDelta()
    {
        return Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;
    }

    public float GetMouseScroll()
    {
        return Mouse.current != null ? Mouse.current.scroll.y.ReadValue() : 0f;
    }

    public bool IsActionPressed(string actionName)
    {
        return inputActions.Player[actionName].IsPressed();
    }

    public bool IsActionTriggered(string actionName)
    {
        return inputActions.Player[actionName].WasPressedThisFrame();
    }

    public bool IsActionReleased(string actionName)
    {
        return inputActions.Player[actionName].WasReleasedThisFrame();
    }

    public Vector2 GetMovement()
    {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }
}
