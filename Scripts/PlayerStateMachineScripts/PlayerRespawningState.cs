using UnityEngine; 

public class PlayerRespawningState : PlayerBaseState
{

    private bool respawnDone  = false; 

    public PlayerRespawningState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Respawning State.");
        _ctx.disableCrossHair(); 
        _ctx.disableInfoUI(); 
        
        respawnDone = false;
        Respawner.instance.Respawn();
        respawnDone = true; 
    }

    public override void UpdateState() {
        CheckSwitchStates();
    }

    public override void FixedUpdateState() { }

    public override void ExitState()
    {
        Debug.Log("Exiting Respawning State.");
        _ctx.shouldRespawn = false; 
    }

    public override void CheckSwitchStates() { 
        if(respawnDone == true ){
           SwitchState(_factory.Grounded());
       }

    }

    public override void InitializeSubState() { }

    
}
