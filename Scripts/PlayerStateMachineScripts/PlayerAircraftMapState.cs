using UnityEngine; 
public class PlayerAircraftMapState: PlayerBaseState{

    public PlayerAircraftMapState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { 
            _isRootState = false; 
        }

    public override void EnterState() {
        Debug.Log("We just entered map mode.");
    }

    public override void ExitState() {
        // Implementation here
    }

    public override void UpdateState() {
        // Implementation here

        CheckSwitchStates();
    }

    public override void CheckSwitchStates() {
        // Implementation here
    }

    public override void InitializeSubState() {
        // Implementation here
    }

}