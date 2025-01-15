using System;
using UnityEngine;

public class PlayerAircraftPilotState : PlayerBaseState
{
    private float forwardVelocity = 10f; // Initial forward velocity
    private  float minForwardVelocity = 10f; // Minimum forward velocity
    private  float maxForwardVelocity = 30f; // Maximum forward velocity
    

    private float yawAngle = 0f; // Current yaw angle
    private float pitchAngle = 0f; // Current pitch angle
    private float yawSpeed = 60f; // Degrees per second for yaw
    private float pitchSpeed = 30f; // Degrees per second for pitch
    private  float maxPitchAngle = 40f; // Max pitch angle

    private float stabilizationSpeed = 2f;

    public PlayerAircraftPilotState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _isRootState = false;
        forwardVelocity = currentContext.DefaultFlightSpeed;
        minForwardVelocity = currentContext.MinimumFlightSpeed;
        maxForwardVelocity = currentContext.MaximumFlightSpeed;

        
    }

    public override void EnterState()
    {
        Debug.Log("Entering PlayerPilotState");

        // Initialize yaw and pitch angles based on current rotation
        Vector3 euler = _ctx.gameObject.transform.eulerAngles;
        yawAngle = euler.y;
        pitchAngle = euler.x;
    }

    public override void UpdateState()
    {
        HandleInput();
        ApplyTilt();
        ApplyMovement();
        ApplyStabilization();
        CheckSwitchStates();
    }

    private void HandleInput()
    {
        // Retrieve input from context
        float inputX = _ctx.CurrentMovementInputX; // Left/right for yaw
        float inputY = _ctx.CurrentMovementInputY; // Up/down for pitch

        // Update yaw and pitch angles based on input
        yawAngle += inputX * yawSpeed * Time.deltaTime;
        pitchAngle -= inputY * pitchSpeed * Time.deltaTime; // Inverted for typical aircraft controls

        // Clamp the pitch angle to prevent excessive tilting
        pitchAngle = Mathf.Clamp(pitchAngle, -maxPitchAngle, maxPitchAngle);

        // Handle acceleration and braking
        if (_ctx.IsRunPressed)
        {
            forwardVelocity += 10f * Time.deltaTime; // Accelerate
        }
        if (_ctx.IsJumpPressed)
        {
            forwardVelocity -= 15f * Time.deltaTime; // Brake
        }

        // Clamp forward velocity within defined limits
        forwardVelocity = Mathf.Clamp(forwardVelocity, minForwardVelocity, maxForwardVelocity);
    }

    private void ApplyTilt()
    {
        // Define target rotation based on yaw and pitch angles
        Quaternion targetRotation = Quaternion.Euler(pitchAngle, yawAngle, 0f);

        // Smoothly interpolate to the target rotation
        _ctx.gameObject.transform.rotation = Quaternion.Slerp(
            _ctx.gameObject.transform.rotation,
            targetRotation,
            Time.deltaTime * stabilizationSpeed
        );
    }

    private void ApplyMovement()
    {
        // Calculate the forward direction based on the current rotation
        Vector3 forwardDirection = _ctx.gameObject.transform.forward;
        forwardDirection.Normalize();

        // Calculate the movement vector
        Vector3 movement = forwardDirection * forwardVelocity;

        // Set the AppliedMovement properties
        _ctx.AppliedMovementX = movement.x;
        _ctx.AppliedMovementY = movement.y; // Typically zero unless vertical movement is desired
        _ctx.AppliedMovementZ = movement.z;
    }

    private void ApplyStabilization()
    {
        // Only stabilize pitch if there's minimal or no pitch input
        if (Mathf.Abs(_ctx.CurrentMovementInputY) < 0.1f)
        {
            // Gradually bring pitch angle back to 0
            pitchAngle = Mathf.MoveTowards(pitchAngle, 0f, stabilizationSpeed * Time.deltaTime);
        }

        // Similarly, stabilize yaw if desired (e.g., to prevent drift)
        // Uncomment the following lines if yaw stabilization is needed
        /*
        if (Mathf.Abs(_ctx.CurrentMovementInputX) < 0.1f)
        {
            yawAngle = Mathf.MoveTowardsAngle(yawAngle, 0f, stabilizationSpeed * Time.deltaTime);
        }
        */
    }

    public override void ExitState()
    {
        Debug.Log("Exiting PlayerPilotState");
    }

    public override void CheckSwitchStates()
    {
        // Add conditions for switching to other states if needed
    }

    public override void InitializeSubState()
    {
        // No sub-state required for pilot mode
    }
}
