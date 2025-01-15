using System.Collections.Generic;

public class PlayerStateFactory{

    private PlayerStateMachine _context; 

    public PlayerStateFactory(PlayerStateMachine currentContext){
        _context = currentContext;
    }

 


    public PlayerBaseState Idle(){
        return new PlayerIdleState(_context, this);
    }
    public PlayerBaseState Walk(){
        return new PlayerWalkState(_context, this);
    }
    public PlayerBaseState Run(){
        return new PlayerRunState(_context,this);
    }

    public PlayerBaseState Jump(){
        return new PlayerJumpState(_context, this);
    }
    public PlayerBaseState Grounded(){
        return new PlayerGroundedState(_context, this);
    }

    public PlayerBaseState Fall(){
        return new PlayerFallState(_context, this);
    }
    public PlayerBaseState Interaction()
    {
        return new PlayerInteractionState(_context, this);
    }
    public PlayerBaseState CollectibleInteraction()
    {
        return new CollectibleInteractionState(_context, this);
    }
    public PlayerBaseState CageBirdInteraction()
    {
        return new CageBirdInteractionState(_context, this);
    }
    public PlayerBaseState Aircraft(){
        return new PlayerAircraftState(_context, this);
    }
    public PlayerBaseState AircraftPilot(){
        return new PlayerAircraftPilotState(_context, this);
    }

    public PlayerBaseState AircraftMap(){
        return new PlayerAircraftMapState(_context, this);
    }
    public PlayerBaseState Cutscene(){
        return new PlayerCutsceneState(_context, this);
    }
    public PlayerBaseState AirJump(){
        return new PlayerAirJumpState(_context, this);
    }
    public PlayerBaseState GroundAim(){
        return new PlayerGroundAimState(_context, this);
    }
    public PlayerBaseState PlayerCutsceneInteractive(){
        return new PlayerCutsceneInteractiveState(_context, this);
    }

    public PlayerBaseState Respawning(){
        return new PlayerRespawningState(_context, this);
    }

}