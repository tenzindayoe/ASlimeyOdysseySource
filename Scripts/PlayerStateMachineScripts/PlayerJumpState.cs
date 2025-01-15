using System.Collections;
using UnityEngine; 

public class PlayerJumpState: PlayerBaseState{

    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){
        _isRootState = true;
    }

    public float timeMax = 1f; 
    public float timeElapsed = 0f;
    public override void EnterState(){
        HandleJump(); 
        
    }
    public  override void UpdateState(){
        _ctx.HandleRotation();
        // HandleGravity();
        CheckSwitchStates();
        timeElapsed += Time.deltaTime ; 
        if(timeElapsed >= timeMax){
            SwitchState(_factory.Fall());
        }
    }

    public override void FixedUpdateState()
    {
        HandleGravity();
    }

    public override  void ExitState(){
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsJumping, false);
        if(_ctx.IsJumpPressed){
            _ctx.RequireNewJumpPress = true; 
        }
        _ctx.CurrentJumpRoutine = _ctx.StartCoroutine(IJumpResetRoutine());
        if(_ctx.JumpCount == 3){
            _ctx.JumpCount = 0;
            _ctx.CharacterAnimator.SetInteger(_ctx.HashJumpCount, _ctx.JumpCount);
        }
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsJumping, false);
    }

    public override  void CheckSwitchStates(){

        if(_ctx.PlayerCharacterController.isGrounded){
            SwitchState(_factory.Grounded());
        } else if(_ctx.getGroundDistanceFromFoot() <=0.5f && _ctx.GetGroundSlope() > _ctx.slopeLimit){
            SwitchState(_factory.Fall());
        }
    }
    public override  void InitializeSubState(){
        if(!_ctx.IsMovementPressed && !_ctx.IsRunPressed){
            SetSubState(_factory.Idle());
        }else if(_ctx.IsMovementPressed && !_ctx.IsRunPressed){
            SetSubState(_factory.Walk());
        }else {
            SetSubState(_factory.Run());
        }
    }

    private void HandleJump(){

        if(_ctx.JumpCount< _ctx.MaxJumps && _ctx.CurrentJumpRoutine != null){
            _ctx.StopCoroutine(_ctx.CurrentJumpRoutine);
        }
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsJumping, true);
        _ctx.IsJumping = true;
        _ctx.JumpCount +=1;
        _ctx.CharacterAnimator.SetInteger(_ctx.HashJumpCount, _ctx.JumpCount);
        _ctx.CurrentMovementY = _ctx.InitialJumpVelocities[_ctx.JumpCount] ; 
        _ctx.AppliedMovementY = _ctx.InitialJumpVelocities[_ctx.JumpCount] ;
    }
    private IEnumerator IJumpResetRoutine()
    {
        yield return new WaitForSeconds(2f);
        _ctx.JumpCount  = 0;
    }
    private void HandleGravity()
    {
        bool isFalling = _ctx.CurrentMovementY <= 0.0f || !_ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;
        if (isFalling)
        {
            float previousYVelocity = _ctx.CurrentMovementY;
            _ctx.CurrentMovementY = _ctx.CurrentMovementY + (_ctx.JumpGravities[_ctx.JumpCount] * fallMultiplier * Time.deltaTime);
            _ctx.AppliedMovementY = Mathf.Max((previousYVelocity  + _ctx.CurrentMovementY)* 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = _ctx.CurrentMovementY;
            _ctx.CurrentMovementY = _ctx.CurrentMovementY + (_ctx.JumpGravities[_ctx.JumpCount] * Time.deltaTime);
            _ctx.AppliedMovementY = (previousYVelocity + _ctx.CurrentMovementY)* 0.5f;
        }
    }
 
}