using System;
using UnityEngine; 
public class PlayerAircraftState : PlayerBaseState{

  
    public PlayerAircraftState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){
        _isRootState = true;
    }
    public override void EnterState(){
        Debug.Log("We are in the aircraft now");
        handleAircraftSetup();
        InitializeSubState();
    }

    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void ExitState(){
        handleAircraftExit();
    }
    public override void CheckSwitchStates(){
        //basically check the lake interaction here. 
    }
    public override void InitializeSubState(){
        //directly go into pilot mode.
        //SetSubState()
        PlayerBaseState pilotState = _factory.AircraftPilot();
        SetSubState(pilotState);
        pilotState.EnterState();
    }
    public void handleAircraftSetup(){
        // set the aircraft game object as the child of the ctx game object
        _ctx.AircraftGameObject.transform.SetParent(_ctx.transform);
        _ctx.AircraftGameObject.transform.localPosition = Vector3.zero;
	}
	public void handleAircraftExit(){
        _ctx.AircraftGameObject.transform.SetParent(_ctx.transform.parent);
	}
}