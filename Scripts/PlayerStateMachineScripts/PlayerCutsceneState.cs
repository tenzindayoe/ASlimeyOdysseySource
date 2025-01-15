using System.Collections.Generic;
using UnityEngine; 

public class PlayerCutsceneState : PlayerBaseState{
    public PlayerCutsceneState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){
        _isRootState = true; 
    }

    public override void EnterState(){
        _ctx.disableInfoUI(); 
        _ctx.disableCrossHair();
        Debug.Log("We have entered the cutscene state");
        //disable all animations 
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsWalking, false);
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsRunning, false);
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsJumping, false);
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsFalling, false);
        

    }
    public override void UpdateState(){
        
        
    }
    public override void ExitState(){
        _ctx.enableInfoUI(); 
        Debug.Log("We have exited the cutscene state");

    }
    public override void CheckSwitchStates(){
        //transition is handled externally.
        
    }
    public override void InitializeSubState(){

    }
}