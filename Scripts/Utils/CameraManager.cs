using UnityEngine;

/// <summary>
/// 相机管理器 - 负责相机视角控制（缩放、旋转）
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public Camera mainCamera;
    public Transform target;
    
    [Header("相机参数")]
    public float distance = 10f;
    public float zoomSpeed = 2f;
    public float rotationSpeed = 2f;
    public float minDistance = 5f;
    public float maxDistance = 20f;
    public float minRotationX = 10f;
    public float maxRotationX = 80f;

    private float currentRotationX = 45f;
    private float currentRotationY = 0f;

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
    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleCameraControls();
    }

    /// <summary>
    /// 处理相机控制
    /// </summary>
    private void HandleCameraControls()
    {
        // 右键拖动旋转
        if (Input.GetMouseButton(1))
        {
            currentRotationY += Input.GetAxis("Mouse X") * rotationSpeed;
            currentRotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentRotationX = Mathf.Clamp(currentRotationX, minRotationX, maxRotationX);
        }

        // 滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        UpdateCameraPosition();
    }

    /// <summary>
    /// 更新相机位置
    /// </summary>
    private void UpdateCameraPosition()
    {
        if (target == null) return;
        
        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);
        Vector3 position = rotation * new Vector3(0f, 0f, -distance) + target.position;

        mainCamera.transform.rotation = rotation;
        mainCamera.transform.position = position;
    }

    /// <summary>
    /// 设置相机目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// 重置相机位置
    /// </summary>
    public void ResetCamera()
    {
        currentRotationX = 45f;
        currentRotationY = 0f;
        distance = 10f;
        UpdateCameraPosition();
    }

    /// <summary>
    /// 放大
    /// </summary>
    public void ZoomIn()
    {
        distance = Mathf.Max(distance - zoomSpeed, minDistance);
        UpdateCameraPosition();
    }

    /// <summary>
    /// 缩小
    /// </summary>
    public void ZoomOut()
    {
        distance = Mathf.Min(distance + zoomSpeed, maxDistance);
        UpdateCameraPosition();
    }

    /// <summary>
    /// 平滑移动到指定位置
    /// </summary>
    public void SmoothMoveTo(Vector3 targetPosition, float duration = 1f)
    {
        StartCoroutine(SmoothMoveCoroutine(targetPosition, duration));
    }

    private System.Collections.IEnumerator SmoothMoveCoroutine(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = mainCamera.transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // 使用平滑插值
            t = Mathf.SmoothStep(0f, 1f, t);
            
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        mainCamera.transform.position = targetPosition;
    }
}
