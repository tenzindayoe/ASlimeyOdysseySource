using UnityEngine;

public class CageBirdInteractionState : PlayerBaseState{
    public CageBirdInteractionState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory){
    
    }
    private bool CageBirdInteractionDone = false;
    public override void EnterState(){
        var interactionState = (PlayerInteractionState) _currentSuperState;
        interactionState.CurrentInteractionObject.Interact();
        interactionState.CurrentInteractionObject.OnAnimationComplete += ()=> {
            CageBirdInteractionDone = true;
        };
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

    }
    public override void ExitState()
    {
        
    }
    public override void CheckSwitchStates()
    {
        if (CageBirdInteractionDone){
        PlayerInteractionState currentSuperState = _currentSuperState as PlayerInteractionState;
        currentSuperState.CurrentInteractionDone = true;
        }
        
    }
    public override void InitializeSubState()
    {
    }
}