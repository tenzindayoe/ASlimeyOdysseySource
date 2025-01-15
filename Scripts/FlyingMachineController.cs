using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FlyingMachineController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActionAsset;

    [Header("Movement Parameters")]
    [SerializeField] private float accelerationSpeed = 20f;
    [SerializeField] public float maxSpeed = 80f;
    [SerializeField] private float brakingForce = 10f;
    [SerializeField] private float liftForceMultiplier = 2.5f;
    [SerializeField] private float dragMultiplier = 0.05f;

    [Header("Rotation Control")]
    [Tooltip("Max pitch rate in degrees/second")]
    [SerializeField] private float maxPitchRate = 60f;
    [Tooltip("Max roll rate in degrees/second")]
    [SerializeField] private float maxRollRate = 100f;

    [Header("Damping and Stability")]
    [Tooltip("Torque factor to achieve desired angular velocity (P gain)")]
    [SerializeField] private float angularVelocityGain = 10f;
    [Tooltip("Torque factor to damp angular velocity changes (D gain)")]
    [SerializeField] private float angularVelocityDamping = 2f;

    // Input actions
    private InputAction accelerateAction;
    private InputAction brakeAction;
    private InputAction tiltAction;


    private Rigidbody rb;

    // Desired angular velocities (in radians/sec)
    private float targetPitchRate = 0f;  // rotation around X-axis
    private float targetRollRate = 0f;   // rotation around Z-axis

    public float CurrentSpeed { get; private set; } // Tracks the current speed of the plane

    private const float Deg2Rad = Mathf.PI / 180f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // Enable gravity for natural climbing/diving behavior
    }

    private void OnEnable()
    {
        if (inputActionAsset == null)
        {
            Debug.LogError("Input Action Asset is not assigned!");
            return;
        }

        accelerateAction = inputActionAsset.FindAction("Accelerate");
        brakeAction = inputActionAsset.FindAction("Brake");
        tiltAction = inputActionAsset.FindAction("Tilt");

        accelerateAction?.Enable();
        brakeAction?.Enable();
        tiltAction?.Enable();
    }

    private void OnDisable()
    {
        accelerateAction?.Disable();
        brakeAction?.Disable();
        tiltAction?.Disable();
    }

    private void FixedUpdate()
    {
        HandleAccelerationAndDrag();
        UpdateTargetAngularRatesFromInput();
        ApplyRotationControl();
        ApplyAerodynamicForces();

        // Update current speed
        CurrentSpeed = rb.linearVelocity.magnitude;
    }

    private void HandleAccelerationAndDrag()
    {
        float accelerationInput = accelerateAction.ReadValue<float>();
        float brakingInput = brakeAction.ReadValue<float>();

        // Thrust along the plane's forward direction (+Z)
        if (rb.linearVelocity.magnitude < maxSpeed && accelerationInput > 0f)
        {
            rb.AddForce(transform.forward * accelerationInput * accelerationSpeed, ForceMode.Acceleration);
        }

        // Braking force opposite to current velocity
        if (brakingInput > 0f && rb.linearVelocity.magnitude > 0.1f)
        {
            rb.AddForce(-rb.linearVelocity.normalized * brakingForce, ForceMode.Acceleration);
        }

        // Quadratic drag (proportional to velocity squared)
        rb.AddForce(-rb.linearVelocity * dragMultiplier * rb.linearVelocity.magnitude, ForceMode.Force);
    }

    private void UpdateTargetAngularRatesFromInput()
    {
        Vector2 tiltInput = tiltAction.ReadValue<Vector2>();

        // Map input to desired angular rates
        float desiredPitchDegPerSec = -tiltInput.y * maxPitchRate; // Nose down if input.y positive
        float desiredRollDegPerSec = tiltInput.x * maxRollRate;    // Roll right if input.x positive

        targetPitchRate = desiredPitchDegPerSec * Deg2Rad;
        targetRollRate  = desiredRollDegPerSec * Deg2Rad;
    }

    private void ApplyRotationControl()
    {
        Vector3 localAngularVel = transform.InverseTransformDirection(rb.angularVelocity);

        float currentPitchRate = localAngularVel.x;
        float currentRollRate  = localAngularVel.z;

        float pitchRateError = targetPitchRate - currentPitchRate;
        float rollRateError  = targetRollRate - currentRollRate;

        // Proportional torque
        Vector3 pitchTorque = transform.right * (pitchRateError * angularVelocityGain);
        Vector3 rollTorque = transform.forward * (rollRateError * angularVelocityGain);

        // Damping torque (derivative)
        Vector3 pitchDampingTorque = -transform.right * (currentPitchRate * angularVelocityDamping);
        Vector3 rollDampingTorque = -transform.forward * (currentRollRate * angularVelocityDamping);

        Vector3 totalTorque = pitchTorque + rollTorque + pitchDampingTorque + rollDampingTorque;
        rb.AddTorque(totalTorque, ForceMode.Force);
    }

    private void ApplyAerodynamicForces()
    {
        // Lift is perpendicular to the wings (along the plane's local up axis)
        float speed = rb.linearVelocity.magnitude;
        float liftForce = speed * liftForceMultiplier; // More speed generates more lift
        rb.AddForce(transform.up * liftForce, ForceMode.Force);

        // Gravity will naturally pull the plane down, so tilting up/down affects climb/dive behavior
    }
}
