using UnityEngine; 
using System.Collections.Generic;
using UnityEngine.XR;
using System.ComponentModel;


public class PlayerAirJumpState : PlayerBaseState
{
	public PlayerAirJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
		: base(currentContext, playerStateFactory) { 
            _isRootState = true; 
        }

    public float timeMax = 1f; 
    public float timeElapsed = 0f;
    private void HandleJump(){

        _ctx.CharacterAnimator.SetBool(_ctx.HashIsJumping, true);
        _ctx.IsJumping = true;
        _ctx.JumpCount +=1;
        
        _ctx.CurrentMovementY = _ctx.InitialJumpVelocities[_ctx.JumpCount] ; 
        _ctx.AppliedMovementY = _ctx.InitialJumpVelocities[_ctx.JumpCount] ;
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

	public override void EnterState()
	{   
        
		HandleJump();
        _ctx.RequireNewJumpPress = true;
        GameAudioManager.Instance.PlayAudio(GameAudioManager.Instance.jumpSound, GameAudioManager.Instance.sfxMixerGroup, true, _ctx.transform.position); 
        Debug.Log("Jump sound played");
        _ctx.playerVFXManager.PlayJump(_ctx.lastGroundPosition + new Vector3(0, _ctx.playerGroundOffset, 0 )); 
    
        
	}


	public override void UpdateState()
	{
        _ctx.HandleRotation();
        HandleGravity();
        
        handleAnimation();
        timeElapsed += Time.deltaTime ; 
        
        CheckSwitchStates();
		// Implementation for updating the state
	}

    private void handleAnimation(){
        // if(_ctx.CurrentMovementY < 0){
        //     _ctx.CharacterAnimator.SetBool(_ctx.HashIsJumping, false);
        //     _ctx.CharacterAnimator.SetBool(_ctx.HashIsFalling, true);
        // }

        // we might have to start playing fall animation once the jump animation is done.
        
    }
	public override void ExitState()
	{
		//_ctx.CharacterAnimator.SetBool(_ctx.HashIsJumping, false);
        
            //_ctx.CharacterAnimator.SetBool(_ctx.HashIsFalling, false);
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsJumping, false);
            

        

        
	}

	public override void CheckSwitchStates()
	{
        
        if(_ctx.shouldRespawn){
            SwitchState(_factory.Respawning());
            return;
        }
		// Implementation for checking switch states
        // if(_ctx.PlayerCharacterController.isGrounded){
        if(_ctx.IsGrounded()){
            SwitchState(_factory.Grounded());
            return ; 
        }
        else if(timeElapsed >= timeMax ){
            SwitchState(_factory.Fall());
            return; 
        }
        // else if(_ctx.CurrentMovementY < 0 ){
        //     SwitchState(_factory.Fall());
        // }
        // // else if (_ctx.IsJumpPressed && !_ctx.RequireNewJumpPress && _ctx.JumpCount < _ctx.MaxJumps){
        // //     SwitchState(_factory.AirJump());
        // // }


	}

	public override void InitializeSubState()
	{
		// Implementation for initializing sub-state
        // if(!_ctx.IsMovementPressed && !_ctx.IsRunPressed){
        //     SetSubState(_factory.Idle());
        // }else if(_ctx.IsMovementPressed && !_ctx.IsRunPressed){
        //     SetSubState(_factory.Walk());
        // }else {
        //     SetSubState(_factory.Run());
        // }
	}
}


