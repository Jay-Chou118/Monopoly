using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public Camera mainCamera;
    public Transform target;
    public float distance = 10f;
    public float zoomSpeed = 2f;
    public float rotationSpeed = 2f;
    public float minDistance = 5f;
    public float maxDistance = 20f;

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

    private void HandleCameraControls()
    {
        if (Input.GetMouseButton(1))
        {
            currentRotationY += Input.GetAxis("Mouse X") * rotationSpeed;
            currentRotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentRotationX = Mathf.Clamp(currentRotationX, 10f, 80f);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        if (target != null)
        {
            Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);
            Vector3 position = rotation * new Vector3(0f, 0f, -distance) + target.position;

            mainCamera.transform.rotation = rotation;
            mainCamera.transform.position = position;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ResetCamera()
    {
        currentRotationX = 45f;
        currentRotationY = 0f;
        distance = 10f;
        UpdateCameraPosition();
    }

    public void ZoomIn()
    {
        distance = Mathf.Max(distance - zoomSpeed, minDistance);
        UpdateCameraPosition();
    }

    public void ZoomOut()
    {
        distance = Mathf.Min(distance + zoomSpeed, maxDistance);
        UpdateCameraPosition();
    }
}
