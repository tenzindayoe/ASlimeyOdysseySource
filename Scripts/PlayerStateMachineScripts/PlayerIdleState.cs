using UnityEngine; 

public class PlayerIdleState: PlayerBaseState{

    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){}

    public override void EnterState(){
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsRunning, false);
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsWalking, false);
        _ctx.AppliedMovementX = 0 ; 
        _ctx.AppliedMovementZ = 0 ;
    }
    public  override void UpdateState(){
        CheckSwitchStates();
    }
    
    public override  void ExitState(){}

    public override  void CheckSwitchStates(){

        if(_ctx.IsMovementPressed && _ctx.IsRunPressed){
            SwitchState(_factory.Run());
        }else if(_ctx.IsMovementPressed ){
            SwitchState(_factory.Walk());
        } 
    }
    public override  void InitializeSubState(){
    }
}