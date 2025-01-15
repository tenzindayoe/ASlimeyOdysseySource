using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Animations.Rigging;
using System.Collections;
using Unity.VisualScripting;

public enum BaseStartState{
    Grounded, 
    Aircraft, 
    Cutscene,
    InteractiveCutscene
}
public class PlayerStateMachine : MonoBehaviour
{




    [Header("Debug options")]

    public bool showCurrentState = false;
    public TextMeshProUGUI currentStateLabel; 

    


    [Header("Player Components")]
    public PlayerInput PlayerInputComponent;
    public CharacterController PlayerCharacterController;
    public Animator CharacterAnimator;
    public Transform followTarget; 
    public Transform raycastObject;

    public LayerMask raycastIgnoreLayer; 

    public Rig PlayerRig; 
    public WaterGunController waterGunController;

    [Header("Camera Settings")]
    public Camera PlayerCamera;

    [Header("Movement Settings")]
    public float WalkMultiplier = 2.0f;
    public float RunMultiplier = 5.0f;
    public float RotationFactor = 10.0f;



    public float slopeLimit = 40; 
    [Header("Respawn settings")]
    public bool shouldRespawn = false; 
    [Header("Gun Settings")]
    public bool gunIsEnabled = true; 
    [Header("Animation Event Settings")]
    [SerializeField]
    private AnimationEventManager _animationEventManager;
    //geters and setter
    public AnimationEventManager AnimEventManager  { get { return _animationEventManager; }}

    [Header("Cutscene Management")]
    [SerializeField]
    private CutsceneManager _cutSceneManager;
    public CutsceneManager GetCutsceneManager { get { return _cutSceneManager; } }

    [Header("Aircraft Reference")]
    [SerializeField]
    private GameObject aircraftGameObject ; 
    public GameObject AircraftGameObject { get { return aircraftGameObject; } set { aircraftGameObject = value; } }

    [Header("Starting State")]
    public BaseStartState baseStartState = BaseStartState.Grounded;

    [Header("Interaction Settings")]
    public float InteractionRadius = 2.0f;
    public float InteractionDistance = 0.5f;

    [Tooltip("A guard so the player doesn't fall too fast.")]
    public float MaxDownfallSpeed = -30f; 

    [Header("Jump Settings")]
    public float MaxJumpHeight = 1.0f;
    public float MaxJumpTime = 0.5f;
    // public float GroundedGravity = -0.05f;  // Very small downward force
    public float Gravity = -9.8f;           // Will be recalculated
    private bool _isJumpPressed = false;

    public bool IsJumpPressed { get { return _isJumpPressed; } }
    private bool _isJumping = false;

    private int _jumpCount = 0;
    private int _maxJumps = 3;
    private bool requireNewJumpPress = false; 

    [Header("Look Settings")]
    public float LookSensitivity = 1.0f;
    public float MaxPitch = 80f;
    public float MinPitch = -80f;

    public float _pitch = 0f;
    public float _yaw = 0f;

    // pFor smooth rotation
    public float RotationSmoothTime = 0.1f;
    public float _currentPitch;
    public float _currentYaw;
    public float _pitchVelocity;
    public float _yawVelocity;

    [Header("Health Settings")]
    public int currentHealth ; 

    [Header("Flight Settings")]
    
    [SerializeField]
    private float _maximumFlightSpeed = 80.0f;
    public float MaximumFlightSpeed { get { return _maximumFlightSpeed; } set { _maximumFlightSpeed = value; } }
    [SerializeField]
    private float _minimumFlightSpeed = 40.0f;
    public float MinimumFlightSpeed { get { return _minimumFlightSpeed; } set { _minimumFlightSpeed = value; } }

    [SerializeField]
    private float _defaultFlightSpeed = 40.0f;
    public float DefaultFlightSpeed { get { return _defaultFlightSpeed; } set { _defaultFlightSpeed = value; } }


    // Animation Hashes
    private int _hashIsWalking;
    private int _hashIsRunning;
    private int _hashIsJumping;
    private int _hashJumpCount;
    private int _hashIsFalling; 

    // Dictionaries for multi-jump
    private Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
    private Dictionary<int, float> _jumpGravities  = new Dictionary<int, float>();
    private Coroutine _currentJumpResetCoroutine = null;


    
    public bool IsJumping { get { return _isJumping; } set { _isJumping = value; } }
    public int JumpCount { get { return _jumpCount; } set { _jumpCount = value; } }

    public bool RequireNewJumpPress { get { return requireNewJumpPress; } set { requireNewJumpPress = value; } }

    public int HashIsWalking { get { return _hashIsWalking; } }
    public int HashIsRunning { get { return _hashIsRunning; } }
    public int HashIsJumping { get { return _hashIsJumping; } }
    public int HashJumpCount { get { return _hashJumpCount; } }

    public int HashIsFalling { get { return _hashIsFalling; } }
    public Dictionary<int, float> InitialJumpVelocities { get { return _initialJumpVelocities; } }
    public Dictionary<int, float> JumpGravities { get { return _jumpGravities; } }

    public float CurrentJumpVelocity { get { return _initialJumpVelocities[_jumpCount]; } }
    public float CurrentJumpGravity { get { return _jumpGravities[_jumpCount]; } }
    public float CurrentJumpTime { get { return MaxJumpTime; } }

    public int MaxJumps { get { return _maxJumps; }}

    public Coroutine CurrentJumpRoutine { get { return _currentJumpResetCoroutine;} set {_currentJumpResetCoroutine = value;}}

    public bool IsMovementPressed { get { return _isMovementPressed; } set { _isMovementPressed = value; } }
    public bool IsRunPressed { get { return _isRunPressed; } set { _isRunPressed = value; } }
    public bool IsInteractPressed { get { return _isInteractPressed; } set { _isInteractPressed = value; } }
    
    public bool _isAimPressed = false; 
    public bool IsAimPressed { get { return _isAimPressed; } set { _isAimPressed = value; } }
    public bool _isFirePressed = false;
    public bool IsFirePressed { get { return _isFirePressed; } set { _isFirePressed = value; } }

    public bool isReloadPressed = false; 
    public bool IsReloadPressed { get { return isReloadPressed; } set { isReloadPressed = value; } }

    private bool _cutsceneMode = false; 
    public bool CutsceneMode { get { return _cutsceneMode; } set { _cutsceneMode = value; }}

    public GameplayCameraManager cameraManager; 




    // Movement & input
    private InputAction _moveAction;
    private InputAction _runAction;
    private InputAction _jumpAction;
    private InputAction _interactAction;

    private InputAction _lookAction;
    private InputAction _aimAction; 
    private InputAction _fireAction; 
    private InputAction _reloadAction; 
    private Vector2 _currentMovementInput;
    private Vector3 _currentMovement;

    private Vector2 _currentLookInput; 

    private bool _isRunPressed;
    private bool _isInteractPressed;    

    private bool _isMovementPressed;

    private Vector3 _appliedMovement; 
    // Jump reset
    
    public float CurrentMovementY {get {return _currentMovement.y; } set {_currentMovement.y = value;}}
    public float AppliedMovementY {get {return _appliedMovement.y; } set {_appliedMovement.y = value;}}    

    private PlayerBaseState _currentPlayerState; 

    public PlayerBaseState CurrentPlayerState  { get{return _currentPlayerState;} set{_currentPlayerState = value; }}
    private PlayerStateFactory _states;

    public float AppliedMovementX {get {return _appliedMovement.x; } set {_appliedMovement.x = value;}}
    public float AppliedMovementZ {get {return _appliedMovement.z; } set {_appliedMovement.z = value;}}

    public float CurrentMovementInputX {get {return _currentMovementInput.x; } set {_currentMovementInput.x = value;}}
    public float CurrentMovementInputY {get {return _currentMovementInput.y; } set {_currentMovementInput.y = value;}}

    public float CurrentLookInputX {get {return _currentLookInput.x; } set {_currentLookInput.x = value;}}
    public float CurrentLookInputY {get {return _currentLookInput.y; } set {_currentLookInput.y = value;}}

    // private GameObject _nearPlayerAircraft = null;
    // public GameObject NearPlayerAircraft { get { return _nearPlayerAircraft; } set { _nearPlayerAircraft = value; } }

    private bool _isAircraftNear = false; 
    public bool IsAircraftNear { get { return _isAircraftNear; } set { _isAircraftNear = value; } }
    
    //for vfx and respwan 
    public Vector3 lastGroundPosition; 
    public Vector3 lastGroundRotation; 

    public float playerGroundOffset = -1f; 

    public PlayerVFXManager playerVFXManager;

    private int GetHitLayerIndex; 

    [Header("UI ")]
    public GameObject InfoUICanvas; 
    public GameObject crossHairImageObj; 

    
    
    private void Awake()
    {
        SetupJumpVariables();
        CacheAnimationHashes();
        _states = new PlayerStateFactory(this);
        // _currentPlayerState = _states.Grounded();
        // _currentPlayerState.EnterState();
        if(baseStartState == BaseStartState.Aircraft){
            SetAircraftAsCurrentState();
        }else if(baseStartState == BaseStartState.Grounded){
            SetGroundedAsCurrentState();
        }
        

        var groundControlMap = PlayerInputComponent.actions.FindActionMap("Ground", true);
        _moveAction = groundControlMap.FindAction("Move", true);
        _runAction  = groundControlMap.FindAction("Run",  true);
        _jumpAction = groundControlMap.FindAction("Jump", true);
        _interactAction = groundControlMap.FindAction("Interact", true);
        _aimAction = groundControlMap.FindAction("Aim", true);
        _fireAction = groundControlMap.FindAction("Fire", true);
        _lookAction = groundControlMap.FindAction("Look", true);
        _reloadAction = groundControlMap.FindAction("Reload", true);

        _jumpAction.started  += OnJump;
        _jumpAction.canceled += OnJump;
        _moveAction.started += OnMove;
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _runAction.started += OnRun;
        _runAction.canceled += OnRun; 
        _aimAction.started += OnAim;
        _aimAction.canceled += OnAim;
        _fireAction.started += OnFire;
        _fireAction.canceled += OnFire;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
        _reloadAction.started += OnReload;
        _reloadAction.canceled += OnReload;


        _interactAction.started += OnInteract;
        _interactAction.canceled += OnInteract;
        //PlayerCharacterController.Move(_appliedMovement * Time.deltaTime);
        GetHitLayerIndex = CharacterAnimator.GetLayerIndex("Hit");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // PlayerCharacterController.Move(_appliedMovement * Time.deltaTime);
        Application.targetFrameRate = 60;   
        SaveStateUtils.StartNewGame(); 
        setCurrentHealthToMax(); 
    }
    
    private void Update()
    {

        //delete rn 
        if(Input.GetKeyDown(KeyCode.F)){
            increaseMaxHealth(2);
        }
        if(Input.GetKeyDown(KeyCode.G)){
            reduceCurrentHealthStatus(1);
        }

        //
        HandleDebugOption();
        // PlayerCharacterController.Move(_appliedMovement * Time.deltaTime);
        _currentPlayerState.UpdateStates();
        if(shouldRespawn == false){
            PlayerCharacterController.Move(_appliedMovement * Time.deltaTime);
        }
      
        if(PlayerCharacterController.isGrounded){
            lastGroundPosition = transform.position; 
            lastGroundRotation = transform.rotation.eulerAngles; 
        }
        
    }

    private void LateUpdate(){
        
        _currentPlayerState.LateUpdateState();
    }

    private void FixedUpdate(){
        _currentPlayerState.FixedUpdateState();

        
        
        
    }
    
    public void setCurrentHealthToMax(){
        int maxHealth = SaveStateUtils.GetMaxHealth(); 
        if (maxHealth == 0 ){
            Debug.LogError("Something went wrong with health");
        }else{
            currentHealth = maxHealth;
            Debug.Log("Health set to max");
        }
    }
    public int GetMaxHealth(){
        return SaveStateUtils.GetMaxHealth();
    }

    public void reduceCurrentHealthStatus(int deltaHealth){

        if(currentHealth + deltaHealth <= 0){
            currentHealth = 0;
        }else{
            currentHealth += deltaHealth;
        }

        if(currentHealth == 0){
            shouldRespawn = true;
        }
    }
    
    public void increaseMaxHealth(int addFactor){
        SaveStateUtils.GetCurrentSaveState().currentMaxHealth += addFactor;
    }

    public void increaseMaxWaterCapacity(int addFactor){
        //transfer
        waterGunController.increaseMaxWaterCapacity(addFactor);
    }

    public void HandleRotation()
{
    // Calculate the direction to look at based on camera-relative movement
    Vector3 camForward = PlayerCamera.transform.forward;
    Vector3 camRight = PlayerCamera.transform.right;

    // Flatten camera vectors (ignore vertical tilt)
    camForward.y = 0f;
    camRight.y = 0f;
    camForward.Normalize();
    camRight.Normalize();

    // Calculate the desired direction based on movement input
    Vector3 desiredMoveDirection = 
        camForward * _currentMovementInput.y + 
        camRight * _currentMovementInput.x;

    // Ensure the direction has significant magnitude to avoid zero-vector issues
    if (desiredMoveDirection.sqrMagnitude > 0.01f)
    {
        // Calculate the target rotation based on the desired direction
        Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);

        // Smoothly rotate towards the target direction
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            RotationFactor * Time.deltaTime
        );
    }
}

    private void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        //RequireNewJumpPress = false;
        if(!_isJumpPressed){
            requireNewJumpPress = false; 
        }
        // the state will be set to true inside the jump air state . 

    }
    private void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0 ;      
        
    }
    


    private void OnInteract(InputAction.CallbackContext context)
    {
        _isInteractPressed = context.ReadValueAsButton();
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        _isAimPressed = context.ReadValueAsButton();
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        isReloadPressed = context.ReadValueAsButton();
    }
    private void OnFire(InputAction.CallbackContext context)
    {
        _isFirePressed = context.ReadValueAsButton();
    }
    
    private void OnLook(InputAction.CallbackContext context)
    {
        _currentLookInput = context.ReadValue<Vector2>();
    }

    public IInteractable GetInteractableInRange(){
        Vector3 InteractionSpherePosition = transform.position + transform.forward * InteractionDistance;
        Collider[] hitColliders = Physics.OverlapSphere(InteractionSpherePosition, InteractionRadius);
        foreach (var collider in hitColliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable) && interactable.IsInteractable)
            {
                return interactable; // Return the first valid interactable
            }
        }
        return null; // No valid interactable found
    }

    public IStoryPointInteractable GetStoryPointInteractableInRange(){
        Vector3 InteractionSpherePosition = transform.position + transform.forward * InteractionDistance;
        Collider[] hitColliders = Physics.OverlapSphere(InteractionSpherePosition, InteractionRadius);
        foreach (var collider in hitColliders)
        {
            if (collider.TryGetComponent(out IStoryPointInteractable interactable) && interactable.IsInteractable())
            {
                return interactable; // Return the first valid interactable
            }
        }
        return null; // No valid interactable found
    }
    
    private void SetupJumpVariables()
    {
        // half of total jump time to apex
        float timeToApex = MaxJumpTime / 2f;

        Gravity = (-2f * MaxJumpHeight) / Mathf.Pow(timeToApex, 2f);
        float baseJumpVelocity = (2f * MaxJumpHeight) / timeToApex;

        // Fill out your dictionary
        float firstJumpGravity  = Gravity; 
        float firstJumpVelocity = baseJumpVelocity;

        float secondJumpGravity = (-2f * (MaxJumpHeight + 0.75f))
                                  / Mathf.Pow(timeToApex * 1.10f, 2f);
        float secondJumpVelocity = (2f * (MaxJumpHeight + 0.75f))
                                   / (timeToApex * 1.10f);

        float thirdJumpGravity = (-2f * (MaxJumpHeight + 1.5f))
                                 / Mathf.Pow(timeToApex * 1.20f, 2f);
        float thirdJumpVelocity = (2f * (MaxJumpHeight + 1.5f))
                                  / (timeToApex * 1.20f);

        _initialJumpVelocities.Add(1, firstJumpVelocity);
        _initialJumpVelocities.Add(2, secondJumpVelocity);
        _initialJumpVelocities.Add(3, thirdJumpVelocity);

        _jumpGravities.Add(0, firstJumpGravity);  // for stepping off ledges
        _jumpGravities.Add(1, firstJumpGravity);
        _jumpGravities.Add(2, secondJumpGravity);
        _jumpGravities.Add(3, thirdJumpGravity);
    }
    private void CacheAnimationHashes()
    {
        _hashIsWalking = Animator.StringToHash("isWalking");
        _hashIsRunning = Animator.StringToHash("isRunning");
        _hashIsJumping = Animator.StringToHash("isJumping");
        _hashJumpCount = Animator.StringToHash("jumpCount");
        _hashIsFalling = Animator.StringToHash("isFalling");
    }

    //mainly for debugging
    // private void OnDrawGizmos()
    // {
    //     if (!Application.isPlaying) return;

    //     // Calculate the position in front of the player
    //     Vector3 interactionSpherePosition = transform.position + transform.forward * InteractionDistance;

    //     // Draw the interaction sphere
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireSphere(interactionSpherePosition, InteractionRadius);

    //     //logic for draw velocity of the plane...
    //     if (this.gameObject == null) return;

    //     // Get the player's position
    //     Vector3 playerPosition = this.gameObject.transform.position;

    //     // Compute the velocity vector directly from the applied movement
    //     Vector3 velocity = new Vector3(AppliedMovementX, AppliedMovementY, AppliedMovementZ);

    //     // Normalize the velocity direction and calculate its magnitude
    //     Vector3 velocityDirection = velocity.normalized;
    //     float velocityMagnitude = velocity.magnitude;

    //     // Scale the velocity for visualization (optional)
    //     Vector3 scaledVelocity = velocityDirection * velocityMagnitude;

    //     // Draw the velocity vector as a green line
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawLine(playerPosition, playerPosition + scaledVelocity);

    //     // Draw a sphere at the endpoint of the velocity vector
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(playerPosition + scaledVelocity, 0.2f);


    // }
    private void OnDrawGizmos()
{
    if (!Application.isPlaying) return;

    // Draw velocity vector
    Vector3 playerPosition = transform.position;
    Vector3 velocity = new Vector3(AppliedMovementX, AppliedMovementY, AppliedMovementZ);
    Vector3 normalizedVelocity = velocity.normalized;

    // Green Line: Represents the velocity vector
    Gizmos.color = Color.green;
    Gizmos.DrawLine(playerPosition, playerPosition + velocity);

    // Red Sphere: Endpoint of the velocity vector
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(playerPosition + velocity, 0.2f);

    // Yellow Line: Normalized velocity direction
    Gizmos.color = Color.yellow;
    Gizmos.DrawLine(playerPosition, playerPosition + normalizedVelocity * 2f);

    // Label Debug Information
#if UNITY_EDITOR
    UnityEditor.Handles.Label(playerPosition + velocity, $"Velocity: {velocity}\nSpeed: {velocity.magnitude:F2}");
#endif
}
    
   


    public void HandleDebugOption(){
        if(showCurrentState){
            currentStateLabel.text = _currentPlayerState.ToString();
        }
        
    }


    //These are used specifically for Initialization and loading save games. 

    public void SetGroundedAsCurrentState(){
        if (_currentPlayerState != _states.Grounded())
        {
            if(_currentPlayerState != null){
                _currentPlayerState.ExitState();
            }
            _currentPlayerState = _states.Grounded();
            _currentPlayerState.EnterState();
        }
    }
    public void SetAircraftAsCurrentState(){
        if(_currentPlayerState != _states.Aircraft())
        {
            if(_currentPlayerState != null){
                _currentPlayerState.ExitState();
            }
            _currentPlayerState = _states.Aircraft();
            _currentPlayerState.EnterState();
        }
    }

    public void SetRespawningAsCurrentState(){
        if(_currentPlayerState != _states.Respawning())
        {
            if(_currentPlayerState != null){
                _currentPlayerState.ExitState();
            }
            _currentPlayerState = _states.Respawning();
            _currentPlayerState.EnterState();
        }
    }

    public void SetCutsceneAsCurrentState(){
        // if(_currentPlayerState != _states.PlayerCutsceneInteractive())
        if(_currentPlayerState != _states.Cutscene())
        {
            if(_currentPlayerState != null){
                _currentPlayerState.ExitState();
            }
            _currentPlayerState = _states.Cutscene();
            _currentPlayerState.EnterState();
        }
    }
    public void SetInteractiveCutsceneAsCurrentState(int id){
        if(_currentPlayerState != null){
            _currentPlayerState.ExitState();
        }
        //we do transition even if it is the same state because for the cutscene we need to set the signal id and it is based on that. 
        _currentPlayerState = _states.PlayerCutsceneInteractive();
        PlayerCutsceneInteractiveState csInter = (PlayerCutsceneInteractiveState)_currentPlayerState;
        csInter.setSignelID(id);
        _currentPlayerState.EnterState();
    }
    public void RecenterThirdPersonCamera(){

        StartCoroutine(cameraManager.instantThirdPersonRecenter());
    }


    
    //setters for state setup
    public void SetPlayerComponents(PlayerInput playerInput, CharacterController characterController, Animator animator, Camera camera){
        PlayerInputComponent = playerInput;
        PlayerCharacterController = characterController;
        CharacterAnimator = animator;
        PlayerCamera = camera;
    }
    public void SetAnimationEventManager(AnimationEventManager animationEventManager){
        _animationEventManager = animationEventManager;
    }
    public void SetCutsceneManager(CutsceneManager cutsceneManager){
        _cutSceneManager = cutsceneManager;
    }
    public void SetAircraftGameObject(GameObject aircraft){
        aircraftGameObject = aircraft;
    }
    public void SetCurrentStateLabel(TextMeshProUGUI currentStateLabel){
        this.currentStateLabel = currentStateLabel;
    }
    public void SetShowCurrentState(bool showCurrentState){
        this.showCurrentState = showCurrentState;
    }

    
    public bool IsGrounded()
    {
        // Define raycast origin and direction
        Vector3 rayOrigin = transform.position  - transform.up* 0.9f; // Slightly above player's feet
        Vector3 rayDirection = Vector3.down;

        RaycastHit hit;
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer); // Create a bitmask that excludes the Player layer

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 1f, layerMask))
        {
            // Calculate slope angle
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            // Check if the slope is within acceptable range
            return PlayerCharacterController.isGrounded && (slopeAngle <= slopeLimit);
        }

        // No ground detected
        return false;
    }

    public bool isAtAFallSlope(){
        return IsGrounded()&&GetGroundSlope()> slopeLimit;
    }

    public float GetGroundSlope()
    {
        // Define raycast origin and direction
        Vector3 rayOrigin = transform.position - transform.up * 0.9f; // Slightly above player's feet
        Vector3 rayDirection = Vector3.down;

        // Perform raycast
        RaycastHit hit;
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer); // Create a bitmask that excludes the Player layer

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 0.2f, layerMask))
        {
            // Return the calculated slope angle
            return Vector3.Angle(hit.normal, Vector3.up);
        }

        // No ground detected, return 0 slope by default
        return 0f;
    }
    public Vector3 GetCloseGroundNormalUnScaled(out float distance){

        Vector3 rayOrigin = transform.position - transform.up * 0.9f; // Slightly above player's feet
        Vector3 rayDirection = Vector3.down;

        // Perform raycast
        RaycastHit hit;
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer); // Create a bitmask that excludes the Player layer

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 2f, layerMask))
        {
            //Debug.Log("Normal detected");
            //draw a line to the normal
            //Debug.DrawRay(hit.point, hit.normal, Color.cyan, 2.0f);
            distance = hit.distance;
            // Return the calculated slope angle
            return hit.normal;
            
        }else{
            //Debug.Log("No normal detected");
        }

        // No ground detected, return 0 slope by default
        distance = 999999999;
        return Vector3.up;

    }

    public float getGroundDistanceFromFoot(){
        Vector3 rayOrigin = transform.position - transform.up * 0.9f; // Slightly above player's feet
        Vector3 rayDirection = Vector3.down;

        // Perform raycast
        RaycastHit hit;
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer); // Create a bitmask that excludes the Player layer

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 2f, layerMask))
        {
            //Debug.Log("Normal detected");
            //draw a line to the normal
            Debug.DrawRay(hit.point, hit.normal, Color.cyan, 2.0f);
            return hit.distance;
        }else{
            //Debug.Log("No normal detected");
        }

        return 999999999f;

    }

    
    // this is kinda like a universal thing so 
    public void GetDamage(){
        // set layer mask "Hit" to 1

        CharacterAnimator.SetLayerWeight(GetHitLayerIndex, 1.0f);
        reduceCurrentHealthStatus(-1);
        CharacterAnimator.SetBool("IsHit", true);
    // Then start a coroutine to reset it after a short delay
        StartCoroutine(ResetHitLayerCoroutine());

        // set layer mask "Hit" to 0 after 0.5f
    }
    private IEnumerator ResetHitLayerCoroutine()
    {
        // Wait 0.5 seconds (tweak as needed)
        yield return new WaitForSeconds(1.3f);// shoudl eb the animation length of the hit animation.
        CharacterAnimator.SetBool("IsHit", false);
        // Set layer weight back to zero
        CharacterAnimator.SetLayerWeight(GetHitLayerIndex, 0.0f);
    }
    

    public void disableInfoUI(){
        InfoUICanvas.SetActive(false);
    }
    public void enableInfoUI(){ 
        InfoUICanvas.SetActive(true);

    }

    public void enableCrossHair(){
        crossHairImageObj.SetActive(true);

    }
    public void disableCrossHair(){
        crossHairImageObj.SetActive(false);
    }

}
