using Unity.VisualScripting;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public float airFriction = 0.1f;
    public float groundFriction = 0.1f;
    private Vector3 startPosition ; 
    private float minFallHeightForSound = 5f; 

    private float fallStateDurationRespawnMax = 10f; 
    private float fallStateDurationRespawn = 0f;
    
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
            _isRootState = true;
            InitializeSubState();
         }

    public override void EnterState()
    {
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsFalling, true);
        startPosition = _ctx.transform.position;

        
        //GameAudioManager.Instance.PlayAudio(GameAudioManager.Instance.fallSound, GameAudioManager.Instance.sfxMixerGroup, true, _ctx.transform.position);
    }
    public override void UpdateState()
    {
      _ctx.HandleRotation();
      fallStateDurationRespawn += Time.deltaTime;

       
    //   HandleGravity();
       CheckSwitchStates(); // should always be at the bottom 
    }
    public override void FixedUpdateState()
    {
       HandleGravity();
          
    }
    public override void ExitState()
    {
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsFalling, false);
        if((startPosition.y - _ctx.transform.position.y) > minFallHeightForSound){
            GameAudioManager.Instance.PlayAudio(GameAudioManager.Instance.fallSound, GameAudioManager.Instance.sfxMixerGroup, true, _ctx.transform.position);
        }
    }
    public override void CheckSwitchStates()
    {
        if(fallStateDurationRespawn >= fallStateDurationRespawnMax){
            _ctx.shouldRespawn = true; 
        }
        if(_ctx.shouldRespawn){
            SwitchState(_factory.Respawning());
            return;
        }
        // if(_ctx.PlayerCharacterController.isGrounded){
        if(_ctx.IsGrounded() ){
            SwitchState(_factory.Grounded());
        }
        // else if (_ctx.IsJumpPressed && !_ctx.RequireNewJumpPress && _ctx.JumpCount < _ctx.MaxJumps){
        //     SwitchState(_factory.AirJump());
        // }
    }
    public override void InitializeSubState()
    {
        // if(!_ctx.IsMovementPressed && !_ctx.IsRunPressed){
        //     SetSubState(_factory.Idle());
        // }else if(_ctx.IsMovementPressed && !_ctx.IsRunPressed){
        //     SetSubState(_factory.Walk());
        // }else {
        //     SetSubState(_factory.Run());
        // }
    }
   
private void HandleGravity()
{
    // Vertical velocity
    float previousYVelocity = _ctx.CurrentMovementY;
    _ctx.CurrentMovementY += _ctx.Gravity * Time.deltaTime; //here ;
    _ctx.AppliedMovementY = Mathf.Max(
        (previousYVelocity + _ctx.CurrentMovementY)*0.5f,
        -20f
    );

    float distance;
    Vector3 groundNormal = _ctx.GetCloseGroundNormalUnScaled(out distance).normalized;

    float angle = Vector3.Angle(groundNormal, Vector3.up);
    // This yields 0 for flat ground, 90 for vertical wall, 180 for upside-down surface

    // Example "downhill factor" in [0..1], bigger factor on steeper slopes
    // e.g. slopeFactor = angle / slopeLimit if slopeLimit=45 => angle=45 => slopeFactor=1.0
    float slopeLimit = _ctx.slopeLimit; 
    float slopeFactor = Mathf.Clamp01(angle / slopeLimit);

    // Optionally clamp angle to something, or invert factor
    // float slopeFactor = 1 - Mathf.Clamp01(angle / 90f); 
    // ... whichever you prefer

    // Add slope-based push
    float pushMagnitude = slopeFactor * 0.5f;  // scale as desired
    _ctx.AppliedMovementX += groundNormal.x * pushMagnitude;
    _ctx.AppliedMovementZ += groundNormal.z * pushMagnitude;

    // Apply some friction or damping
    float horizontalFriction = 0.01f;
    _ctx.AppliedMovementX *= (1f - horizontalFriction);
    _ctx.AppliedMovementZ *= (1f - horizontalFriction);

    // Debug
    Debug.DrawRay(_ctx.transform.position, groundNormal, Color.green);
}



}
