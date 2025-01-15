using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform playerTransform; // Reference to the player's transform
    [SerializeField] private float distanceToPlayer = 10f; // Fixed distance from the player
    [SerializeField] private Vector2 defaultAngle = new Vector2(30f, 0f); // Default pitch (x) and yaw (y) angles
    [SerializeField] private float rotationSpeed = 2f; // Speed for rotating around the player
    [SerializeField] private InputActionAsset inputActionAsset; // Input Action Asset

    private InputAction revolveAction;
    private float currentYaw;
    private float currentPitch;

    private void Awake()
    {
        // Initialize input action
        if (inputActionAsset != null)
        {
            revolveAction = inputActionAsset.FindAction("Revolve");
            if (revolveAction != null)
            {
                revolveAction.Enable();
            }
            else
            {
                Debug.LogError("Revolve action not found in Input Action Asset!");
            }
        }
    }

    private void Start()
    {
        // Set default angles
        currentPitch = defaultAngle.x;
        currentYaw = defaultAngle.y;
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
        UpdateCameraPosition();
    }

    private void HandleCameraRotation()
    {
        if (revolveAction != null)
        {
            // Read input from the Revolve action
            Vector2 input = revolveAction.ReadValue<Vector2>();
            currentYaw += input.x * rotationSpeed;
            currentPitch -= input.y * rotationSpeed;

            // Clamp pitch to avoid flipping the camera
            currentPitch = Mathf.Clamp(currentPitch, -30f, 60f);
        }
    }

    private void UpdateCameraPosition()
    {
        // Calculate the new camera position and rotation
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 offset = rotation * Vector3.back * distanceToPlayer;

        transform.position = playerTransform.position + offset;
        transform.LookAt(playerTransform.position);
    }

    private void OnDisable()
    {
        if (revolveAction != null)
        {
            revolveAction.Disable();
        }
    }
}
