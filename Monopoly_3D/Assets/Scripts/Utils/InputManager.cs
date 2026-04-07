using UnityEngine;

/// <summary>
/// 输入管理器 - 负责处理玩家输入
/// 支持新版 Input System 和旧版 Input Manager
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

#if ENABLE_INPUT_SYSTEM
    private UnityEngine.InputSystem.PlayerInput playerInput;
#endif

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

#if ENABLE_INPUT_SYSTEM
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
#endif
    }

    /// <summary>
    /// 检查鼠标按钮是否按下
    /// </summary>
    public bool IsMouseButtonPressed(int button)
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = UnityEngine.InputSystem.Mouse.current;
        return mouse != null && mouse.leftButton.isPressed;
#else
        return Input.GetMouseButton(button);
#endif
    }

    /// <summary>
    /// 检查按键是否按下
    /// </summary>
    public bool IsKeyPressed(KeyCode key)
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null) return false;
        
        // 将 KeyCode 转换为 Key
        var inputKey = ConvertKeyCodeToKey(key);
        return keyboard[inputKey].isPressed;
#else
        return Input.GetKey(key);
#endif
    }

    /// <summary>
    /// 获取鼠标位置
    /// </summary>
    public Vector2 GetMousePosition()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = UnityEngine.InputSystem.Mouse.current;
        return mouse != null ? mouse.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    /// <summary>
    /// 获取鼠标移动增量
    /// </summary>
    public Vector2 GetMouseDelta()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = UnityEngine.InputSystem.Mouse.current;
        return mouse != null ? mouse.delta.ReadValue() : Vector2.zero;
#else
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
    }

    /// <summary>
    /// 获取鼠标滚轮
    /// </summary>
    public float GetMouseScroll()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = UnityEngine.InputSystem.Mouse.current;
        return mouse != null ? mouse.scroll.y.ReadValue() : 0f;
#else
        return Input.GetAxis("Mouse ScrollWheel");
#endif
    }

    /// <summary>
    /// 获取移动输入
    /// </summary>
    public Vector2 GetMovement()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null) return Vector2.zero;
        
        Vector2 movement = Vector2.zero;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) movement.y += 1;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) movement.y -= 1;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) movement.x -= 1;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) movement.x += 1;
        return movement;
#else
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif
    }

#if ENABLE_INPUT_SYSTEM
    /// <summary>
    /// 将 KeyCode 转换为 Input System 的 Key
    /// </summary>
    private UnityEngine.InputSystem.Key ConvertKeyCodeToKey(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.W: return UnityEngine.InputSystem.Key.W;
            case KeyCode.A: return UnityEngine.InputSystem.Key.A;
            case KeyCode.S: return UnityEngine.InputSystem.Key.S;
            case KeyCode.D: return UnityEngine.InputSystem.Key.D;
            case KeyCode.UpArrow: return UnityEngine.InputSystem.Key.UpArrow;
            case KeyCode.DownArrow: return UnityEngine.InputSystem.Key.DownArrow;
            case KeyCode.LeftArrow: return UnityEngine.InputSystem.Key.LeftArrow;
            case KeyCode.RightArrow: return UnityEngine.InputSystem.Key.RightArrow;
            case KeyCode.Space: return UnityEngine.InputSystem.Key.Space;
            case KeyCode.Return: return UnityEngine.InputSystem.Key.Enter;
            case KeyCode.Escape: return UnityEngine.InputSystem.Key.Escape;
            default: return UnityEngine.InputSystem.Key.None;
        }
    }
#endif
}
