using UnityEngine;
using UnityEngine.UIElements;

public class PlayerGroundedState: PlayerBaseState{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory){
        _isRootState = true; 
        InitializeSubState(); 
    }

    public override void EnterState(){
        _ctx.disableCrossHair(); 
        _ctx.enableInfoUI(); 
        _ctx.CurrentMovementY = _ctx.Gravity;
        _ctx.AppliedMovementY = _ctx.Gravity;
        _ctx.JumpCount = 0; 
        _ctx.waterGunController.startReload(); 
    }
    public  override void UpdateState(){
        _ctx.HandleRotation();
        CheckSwitchStates();
        

       
    }
    
    public override  void ExitState(){
        _ctx.playerVFXManager.StopWalk(); 
        _ctx.playerVFXManager.disableWalkEffect();
        
    }

    public override  void CheckSwitchStates(){
       // Debug.Log($"Current Ground Slope: {_ctx.GetGroundSlope()}");

        if(_ctx.shouldRespawn){
            SwitchState(_factory.Respawning());
            return;
        }
        //this is an exception, this is not a regular cutscene so we just did it like this. This is a location independant cutscene so. 
        if (_ctx.IsAircraftNear && _ctx.IsInteractPressed){
            
           //swtich to the get on aircraft animation
           //player aircraft ride cutscene
            _ctx.CutsceneMode = true;// did this for consistency in the cutscene logic in the cutscene state.
            setGetonAircraft();
            SwitchState(_factory.Cutscene());
            return ; // perform early exit since the switch state condition is met. 
           
        }

        if(_ctx.CutsceneMode){
            SwitchState(_factory.Cutscene());
            return; 
        }

        if(_ctx.IsAimPressed && _ctx.gunIsEnabled){
            SwitchState(_factory.GroundAim());
            return; 
        }

        if (_ctx.IsJumpPressed && !_ctx.RequireNewJumpPress){
            ///SwitchState(_factory.Jump());
            SwitchState(_factory.AirJump());
            return ; 
        
        // } else if(!_ctx.PlayerCharacterController.isGrounded ){
        //}else if(!_ctx.IsGrounded() && !_ctx.PlayerCharacterController.isGrounded){
        }else if(!_ctx.IsGrounded() ){
            SwitchState(_factory.Fall());
            return ; 
        }
        // if(_ctx.IsInteractPressed ){

            //Check if interaction is in the range.
            // if(_ctx.GetInteractableInRange() != null){
            //     SwitchState(_factory.Interaction());
            //     return; 
            // }
        // }

        if(_ctx.GetStoryPointInteractableInRange() != null){

            if(_ctx.GetStoryPointInteractableInRange().IsInteractable() && _ctx.IsInteractPressed){
                _ctx.GetStoryPointInteractableInRange().Interact();
            }
            
        }

        

    }
    public override  void InitializeSubState(){
        
        if(!_ctx.IsMovementPressed && !_ctx.IsRunPressed){
            SetSubState(_factory.Idle());

        }else if(_ctx.IsMovementPressed && !_ctx.IsRunPressed){
            Debug.Log("We are walking");
            SetSubState(_factory.Walk());
        }else {
            SetSubState(_factory.Run());
        }
    }

    private void setGetonAircraft(){
        Debug.Log("Doing the setup needed for get on aircraft");
    }
}