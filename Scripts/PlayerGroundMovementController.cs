using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerGroundMovementController : MonoBehaviour
{
    [Header("Player Components")]
    public PlayerInput PlayerInputComponent;
    public CharacterController PlayerCharacterController;
    public Animator CharacterAnimator;

    [Header("Camera Settings")]
    public Camera PlayerCamera;

    [Header("Movement Settings")]
    public float WalkSpeed = 2.0f;
    public float RunSpeed = 5.0f;
    public float RotationFactor = 10.0f;

    [Tooltip("A guard so the player doesn't fall too fast.")]
    public float MaxDownfallSpeed = -30f; 

    [Header("Jump Settings")]
    public float MaxJumpHeight = 1.0f;
    public float MaxJumpTime = 0.5f;
    public float GroundedGravity = -0.05f;  // Very small downward force
    public float Gravity = -9.8f;           // Will be recalculated
    private bool _isJumpPressed = false;
    private bool _isJumping = false;

    private int _jumpCount = 0;
    private int _maxJumps = 3;
    private bool _isJumpAnimating = false;

    // Animation Hashes
    private int _hashIsWalking;
    private int _hashIsRunning;
    private int _hashIsJumping;
    private int _hashJumpCount;

    // Dictionaries for multi-jump
    private Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
    private Dictionary<int, float> _jumpGravities  = new Dictionary<int, float>();

    // Movement & input
    private InputAction _moveAction;
    private InputAction _runAction;
    private InputAction _jumpAction;
    private Vector2 _currentMovementInput;
    private Vector3 _currentMovement;
    private bool _isWalking;
    private bool _isRunning;

    private Vector3 _appliedMovement; 

    // Jump reset
    private Coroutine currentJumpResetCoroutine = null;

    // -------------------------------------------------------
    // INIT
    // -------------------------------------------------------
    private void Awake()
    {
        SetupJumpVariables();
        CacheAnimationHashes();
    }

    private void Start()
    {
        var groundControlMap = PlayerInputComponent.actions.FindActionMap("Ground", true);
        _moveAction = groundControlMap.FindAction("Move", true);
        _runAction  = groundControlMap.FindAction("Run",  true);
        _jumpAction = groundControlMap.FindAction("Jump", true);

        _jumpAction.started  += OnJump;
        _jumpAction.canceled += OnJump;
    }

    private void OnDestroy()
    {
        _jumpAction.started  -= OnJump;
        _jumpAction.canceled -= OnJump;
    }

    // -------------------------------------------------------
    // UPDATE LOOP
    // -------------------------------------------------------
    private void Update()
    {
        // ORDER IS IMPORTANT:
        HandleJump();
        HandleGravity();
        HandleMovement();
        HandleRotation();
        HandleAnimations();
    }

    // -------------------------------------------------------
    // INPUT
    // -------------------------------------------------------
    private void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    // -------------------------------------------------------
    // JUMPING
    // -------------------------------------------------------
    private void HandleJump()
    {
        // Only jump if grounded. (No mid-air multi-jump in this design.)
        if (PlayerCharacterController.isGrounded)
        {
            // if the jump button is pressed & we're not already jumping
            if (_isJumpPressed && !_isJumping)
            {
                // If we are at max jump, reset to 1, else increment
                if (_jumpCount == _maxJumps)
                {
                    _jumpCount = 1;
                }
                else
                {
                    _jumpCount++;
                }

                // Cancel any countdown to reset jumps
                if (currentJumpResetCoroutine != null)
                {
                    StopCoroutine(currentJumpResetCoroutine);
                }

                // Perform the jump
                _isJumping = true;
                float jumpVel = _initialJumpVelocities[_jumpCount];
                _currentMovement.y = jumpVel;  // set upward velocity
                _appliedMovement.y = jumpVel;
                // Start a new jump reset countdown
                currentJumpResetCoroutine = StartCoroutine(JumpResetRoutine());
            }
            // If the player let go of jump while still grounded, we stop "isJumping"
            else if (!_isJumpPressed && _isJumping)
            {
                _isJumping = false;
            }
        }
    }

    private IEnumerator JumpResetRoutine()
    {
        yield return new WaitForSeconds(2f);
        _jumpCount = 0;
    }

    // -------------------------------------------------------
    // GRAVITY
    // -------------------------------------------------------
    private void HandleGravity()
    {
        // We'll say "falling" if y <= 0 or no longer pressing jump
        bool isFalling = (_currentMovement.y <= 0f || !_isJumpPressed);
        float fallMultiplier = 2.0f;

        if (PlayerCharacterController.isGrounded && !_isJumping)
        {
            // If grounded & not jumping, set a tiny downward force
            _currentMovement.y = GroundedGravity;
            _appliedMovement.y = GroundedGravity;    


            if (_isJumpAnimating)
            {
                CharacterAnimator.SetBool("isJumping", false);
                _isJumpAnimating = false;
            }
        }
        else if (isFalling)
        {
            float prevVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (_jumpGravities[_jumpCount] * fallMultiplier * Time.deltaTime);
            float verletVelocity = (prevVelocity + _currentMovement.y) * 0.5f;
            // clamp so we never exceed maxDownfallSpeed
            _appliedMovement.y = Mathf.Clamp(verletVelocity, MaxDownfallSpeed, 0f);
        }
        else
        {
            // Rising or apex approach
            float prevVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (_jumpGravities[_jumpCount] * Time.deltaTime);
            float verletVelocity = (prevVelocity + _currentMovement.y) * 0.5f;
            _appliedMovement.y = verletVelocity;
        }
    }

    // -------------------------------------------------------
    // MOVEMENT
    // -------------------------------------------------------
    private void HandleMovement()
    {
        // read input
        _currentMovementInput = _moveAction.ReadValue<Vector2>();

        // camera-forward & camera-right (flattened)
        Vector3 camForward = PlayerCamera.transform.forward;
        Vector3 camRight   = PlayerCamera.transform.right;
        camForward.y = 0f;
        camRight.y   = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // combine
        Vector3 desiredMoveDirection = 
            camForward * _currentMovementInput.y +
            camRight   * _currentMovementInput.x;

        // are we moving / running
        _isWalking = (desiredMoveDirection.sqrMagnitude > 0.01f);
        _isRunning = (_isWalking && _runAction.IsPressed());

        float currentSpeed = _isRunning ? RunSpeed : WalkSpeed;

        // combine horizontal + the y we already set in gravity/jump
        Vector3 finalMove = desiredMoveDirection * currentSpeed;

        _appliedMovement.x = finalMove.x;
        _appliedMovement.z = finalMove.z;
        // move character
        PlayerCharacterController.Move(_appliedMovement * Time.deltaTime);

        // store horizontal for rotation
        _currentMovement.x = _appliedMovement.x;
        _currentMovement.z = _appliedMovement.z;
    }

    // -------------------------------------------------------
    // ROTATION
    // -------------------------------------------------------
    private void HandleRotation()
    {
        if (_isWalking)
        {
            Vector3 direction = new Vector3(_currentMovement.x, 0f, _currentMovement.z);
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    RotationFactor * Time.deltaTime
                );
            }
        }
    }

    // -------------------------------------------------------
    // ANIMATIONS
    // -------------------------------------------------------

    private void HandleAnimations()
    {
        bool isWalkingAnimation = CharacterAnimator.GetBool(_hashIsWalking);
        bool isRunningAnimation = CharacterAnimator.GetBool(_hashIsRunning);

        // Walking animation
        if (_isWalking && !isWalkingAnimation)
        {
            CharacterAnimator.SetBool(_hashIsWalking, true);
        }
        else if (!_isWalking && isWalkingAnimation)
        {
            CharacterAnimator.SetBool(_hashIsWalking, false);
        }

        // Running animation
        if (_isWalking && _isRunning && !isRunningAnimation)
        {
            CharacterAnimator.SetBool(_hashIsRunning, true);
        }
        else if ((!_isWalking || !_isRunning) && isRunningAnimation)
        {
            CharacterAnimator.SetBool(_hashIsRunning, false);
        }

        // Jumping animation
        if (_isJumping )
        {
            CharacterAnimator.SetBool(_hashIsJumping, true);
            _isJumpAnimating = true;
            CharacterAnimator.SetInteger(_hashJumpCount, _jumpCount);
        }

        // if (_isJumping && !_isJumpAnimating && !PlayerCharacterController.isGrounded)
        // {
        //     // Start the jumping animation
        //     CharacterAnimator.SetBool(_hashIsJumping, true);
        //     _isJumpAnimating = true;
        //     CharacterAnimator.SetInteger(_hashJumpCount, _jumpCount);
        // }
        // else if (!PlayerCharacterController.isGrounded && !_isJumpPressed)
        // {
        //     // Stop the animation if the player is falling and not pressing the jump button
        //     CharacterAnimator.SetBool(_hashIsJumping, false);
        //     _isJumpAnimating = false;
        // }
        // else if (PlayerCharacterController.isGrounded && _isJumpAnimating)
        // {
        //     // Ensure the jump animation stops when grounded
        //     CharacterAnimator.SetBool(_hashIsJumping, false);
        //     _isJumpAnimating = false;
        //     CharacterAnimator.SetInteger(_hashJumpCount, 0); // Reset jump count in animation
        // }
    }
    // -------------------------------------------------------
    // SETUP JUMP VARIABLES
    // -------------------------------------------------------
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
    }
}
