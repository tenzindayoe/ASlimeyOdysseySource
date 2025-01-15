using System.Collections;
using UnityEngine; 
public class PlayerGroundAimState : PlayerBaseState {
	public PlayerGroundAimState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
		: base(currentContext, playerStateFactory) {
        _isRootState = true ; 
	}
    

    public IEnumerator chargeWaterCoroutine;
	public override void EnterState() {
        _ctx.enableCrossHair(); 
        _ctx.enableInfoUI(); 
		Debug.Log("We just entered the aim state");
        
        //make the player look in the direction of the camera
        Camera cam = _ctx.PlayerCamera;
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();
        _ctx.transform.forward = camForward;

        _ctx.waterGunController.enableGunObject(); 
        //set the wegith fot he layer calle dAIm to 1
        _ctx.CharacterAnimator.SetLayerWeight(_ctx.CharacterAnimator.GetLayerIndex("AimTorso"), 1);
        _ctx.CharacterAnimator.SetBool("isAiming", true);
        _ctx.cameraManager.setCameraMode(CameraMode.Aim);
        //change the cinemacine camera to the aim camera..
        //reset
        Vector3 eulerAngles = _ctx.transform.eulerAngles;
        _ctx._yaw = eulerAngles.y;  // We’ll drive horizontal rotation by yaw

        // If you’re rotating the pitch on a child object (like `followTarget`),
        // read that pitch so you’re synced up. If you prefer pitch to start at 0
        // every time, just set `_ctx._pitch = 0f;`
        Vector3 localEulerAngles = _ctx.followTarget.transform.localEulerAngles;
        // Because localEulerAngles.x can be in [0..360], you might want to 
        // convert it to a -180..180 range before clamping:
        if (localEulerAngles.x > 180f) localEulerAngles.x -= 360f;
        _ctx._pitch = Mathf.Clamp(localEulerAngles.x, _ctx.MinPitch, _ctx.MaxPitch);

        _ctx.PlayerRig.weight = 1;
        // _ctx.waterGunController.chargeWater(); 
        _ctx.waterGunController.stopReload(); 
        doCharge(); 
    
        // set the animator to the aim state
        

	}

	public override void UpdateState() {
		
        HandleLook();
        HandleRayCastFromCenter();
        HandleMovement();
        HandleFire();


        CheckSwitchStates();
	}


    public override void LateUpdateState()
    {
        // HandleLook();
    }
    
    //     public void HandleRayCastFromCenter() {
    //     Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    //     Ray ray = _ctx.PlayerCamera.ScreenPointToRay(screenCenter);

    //     // Calculate the plane on which the player exists
    //     Plane playerPlane = new Plane(Vector3.up, _ctx.transform.position); // Assuming player's plane is horizontal
    //     float distanceToPlane;

    //     // Default endpoint of the ray if nothing is hit
    //     Vector3 rayEndPoint;

    //     // Check if the ray intersects the player's plane
    //     if (playerPlane.Raycast(ray, out distanceToPlane)) {
    //         // Calculate the intersection point on the player's plane
    //         Vector3 planeIntersectionPoint = ray.GetPoint(distanceToPlane);

    //         // Create a new ray from the plane intersection point
    //         Ray adjustedRay = new Ray(planeIntersectionPoint, _ctx.PlayerCamera.transform.forward);
    //         RaycastHit hit;

    //         // Perform the actual raycast
    //         if (Physics.Raycast(adjustedRay, out hit, 100, ~_ctx.raycastIgnoreLayer)) {
    //             rayEndPoint = hit.point; // Use the hit point if something is hit
    //             Debug.Log($"Raycast hit: {hit.collider.name}");
    //         } else {
    //             // If nothing is hit, set the endpoint to the maximum distance
    //             rayEndPoint = adjustedRay.GetPoint(100);
    //             Debug.Log("No valid target hit. Using max distance endpoint.");
    //         }
    //     } else {
    //         // Fallback if the ray doesn't intersect the plane
    //         rayEndPoint = ray.GetPoint(100);
    //         Debug.Log("Ray does not intersect the player's plane. Using max distance endpoint.");
    //     }

    //     // Update the position of the raycast object
    //     _ctx.raycastObject.transform.position = rayEndPoint;
    // }


    public void HandleRayCastFromCenter() {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = _ctx.PlayerCamera.ScreenPointToRay(screenCenter);

        // Calculate the endpoint at a fixed distance along the ray direction
        float fixedDistance = 100f; // Set your desired distance here
        Vector3 rayEndPoint = ray.GetPoint(fixedDistance);

        // Update the position of the raycast object
        _ctx.raycastObject.transform.position = rayEndPoint;

        //Debug.Log($"Raycast point set to fixed distance: {rayEndPoint}");
    }

	public override void ExitState() {
        tryCancelCharge(); 
        _ctx.disableCrossHair(); 
		// Implementation for exiting the state
        Debug.Log("We just exited the aim state");
        _ctx.CharacterAnimator.SetLayerWeight(_ctx.CharacterAnimator.GetLayerIndex("AimTorso"), 0);
        _ctx.CharacterAnimator.SetBool("isAiming", false);
        _ctx.cameraManager.setCameraMode(CameraMode.ThirdPerson);
        _ctx.PlayerRig.weight = 0;
        _ctx.waterGunController.destroyCharged(); 

        _ctx.waterGunController.disableGunObject();
        
        //maybe ? ?? 
        //_ctx.waterGunController.stopFiring();

	}

	public override void CheckSwitchStates() {

        if(_ctx.shouldRespawn){
            SwitchState(_factory.Respawning());
            return;
        }
        
        if(_ctx._isAimPressed == false){
            
            SwitchState(_factory.Grounded());
        }else if(!_ctx.IsGrounded() ){
            SwitchState(_factory.Fall());
        }
        // add the functionality for reload / recall water globules.
	}

    // private void HandleLook()
    // {
    //     // Read look input values
    //     float lookX = _ctx.CurrentLookInputX;
    //     float lookY = _ctx.CurrentLookInputY;

    //     // Apply sensitivity and deltaTime
    //     float deltaX = lookX * _ctx.LookSensitivity * Time.deltaTime;
    //     float deltaY = lookY * _ctx.LookSensitivity * Time.deltaTime;

    //     // Increment yaw and pitch values
    //     _ctx._yaw += deltaX; // Horizontal rotation
    //     _ctx._pitch -= deltaY; // Vertical rotation (invert Y-axis behavior)

    //     // Clamp pitch to prevent over-rotation
    //     _ctx._pitch = Mathf.Clamp(_ctx._pitch, _ctx.MinPitch, _ctx.MaxPitch);

    //     // Incrementally adjust the player's Y-axis rotation (horizontal)
    //     _ctx.transform.rotation = Quaternion.Euler(0f, _ctx.transform.eulerAngles.y + deltaX, 0f);

    //     // Incrementally adjust the follow target's X-axis rotation (vertical)
    //     _ctx.followTarget.transform.localRotation = Quaternion.Euler(
    //         _ctx.followTarget.transform.localEulerAngles.x + deltaY, // Adjust pitch
    //         0f,
    //         0f
    //     );
    // }

    private float rotationSmoothSpeed = 100f; // adjust as desired

private void HandleLook()
{
    // Read look input values
    float lookX = _ctx.CurrentLookInputX;
    float lookY = _ctx.CurrentLookInputY;

    // Compute raw delta rotations (still applying sensitivity & deltaTime)
    float deltaX = lookX * _ctx.LookSensitivity * Time.deltaTime;
    float deltaY = lookY * _ctx.LookSensitivity * Time.deltaTime;

    // Calculate desired yaw/pitch
    float targetYaw   = _ctx._yaw   + deltaX;   // horizontal (y-axis) rotation
    float targetPitch = _ctx._pitch - deltaY;   // vertical (x-axis) rotation (inverted Y)

    // Clamp pitch to prevent over-rotation
    targetPitch = Mathf.Clamp(targetPitch, _ctx.MinPitch, _ctx.MaxPitch);

    // Update our stored yaw & pitch for next frame
    _ctx._yaw = targetYaw;
    _ctx._pitch = targetPitch;

    // Create the target rotations:
    // - Player rotates around the y-axis (yaw)
    // - The followTarget rotates around the x-axis (pitch)
    Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
    Quaternion targetLocalRotation = Quaternion.Euler(targetPitch, 0f, 0f);

    // Smoothly rotate the player
    _ctx.transform.rotation = Quaternion.Slerp(
        _ctx.transform.rotation,
        targetRotation,
        rotationSmoothSpeed * Time.deltaTime
    );

    // Smoothly rotate the follow target
    _ctx.followTarget.transform.localRotation = Quaternion.Slerp(
        _ctx.followTarget.transform.localRotation,
        targetLocalRotation,
        rotationSmoothSpeed * Time.deltaTime
    );
}


    private void HandleMovement(){
        //only moves the character in the x and z axis withtout rotation 

        _ctx.CharacterAnimator.SetFloat("MoveX", _ctx.CurrentMovementInputX);
        _ctx.CharacterAnimator.SetFloat("MoveZ", _ctx.CurrentMovementInputY);


        Vector3 camForward = _ctx.PlayerCamera.transform.forward;
        Vector3 camRight = _ctx.PlayerCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Calculate desired move direction
        Vector3 desiredMoveDirection =
            camForward * _ctx.CurrentMovementInputY +
            camRight * _ctx.CurrentMovementInputX;

        // Apply walking speed multiplier
        float speed = _ctx.WalkMultiplier;

        _ctx.AppliedMovementX = desiredMoveDirection.x * speed;
        _ctx.AppliedMovementZ = desiredMoveDirection.z * speed;

    }
    private void HandleFire(){
        if(_ctx._isFirePressed){
            //fire the water globule
            _ctx.waterGunController.startFiring(); 
        } 

        // else if(_ctx.IsReloadPressed){
        //     _ctx.waterGunController.Reload(); 

        //     if(chargeWaterCoroutine == null){
        //         doCharge(); 
        //     }
            
        // }

     

    }
    private void doCharge(){

        if(chargeWaterCoroutine != null){
            tryCancelCharge();
        }
        
        chargeWaterCoroutine = _ctx.waterGunController.charge(); 
        _ctx.StartCoroutine(chargeWaterCoroutine);

    }

    private void tryCancelCharge(){
        if (chargeWaterCoroutine != null) {
            _ctx.StopCoroutine(chargeWaterCoroutine);
            chargeWaterCoroutine = null; // Nullify the reference to avoid reuse
            Debug.Log("Stopped charge water coroutine.");
        }

    }
	public override void InitializeSubState() {
		// Implementation for initializing sub-states
	}
}