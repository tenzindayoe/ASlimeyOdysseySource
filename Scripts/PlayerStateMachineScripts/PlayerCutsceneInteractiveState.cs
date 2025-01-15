using UnityEngine; 

public class PlayerCutsceneInteractiveState : PlayerBaseState{

    public PlayerCutsceneInteractiveState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){
        _isRootState = true; 
    }
    private int signalID;
    private bool signalSet = false; 


    public void setSignelID(int id){
        signalID = id; 
        signalSet = true;
    }
    public override void EnterState(){
        Debug.Log("We have entered the cutscene state");
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsWalking, false);
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsRunning, false);
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsJumping, false);
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsFalling, false);
    }
    public override void UpdateState(){
        if(!signalSet){
            Debug.LogError("Signal ID not set for the cutscene interactive state");
            return;
        }
        if(_ctx.IsInteractPressed){
            UniSignals.Instance.EmitSignal(signalID);
        }

    }
    public override void ExitState(){
        signalSet = false;
    }
    public override void CheckSwitchStates(){
        //transition is handled externally.
        
    }
    public override void InitializeSubState(){

    }
    

}