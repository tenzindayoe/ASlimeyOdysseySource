using UnityEngine;

public abstract class PlayerBaseState
{
    protected bool _isRootState = false;
    protected PlayerStateMachine _ctx; 
    protected PlayerStateFactory _factory;
    protected PlayerBaseState _currentSuperState;
    protected PlayerBaseState _currentSubState; 
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
        InitializeSubState();
    }
    public abstract void EnterState();
    public abstract void UpdateState();

    //make an overriable virtual method for LateUpdate;
    public virtual void LateUpdateState(){
        //needs to be overriden/implemented in the child class
    }
    //

    public virtual void FixedUpdateState(){
        //needs to be overriden/implemented in the child class
    }


    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates(){
        UpdateState();
        if(_currentSubState != null){
            _currentSubState.UpdateStates();
        }
    }

    protected void SwitchState(PlayerBaseState newState){
        //Debug.Log(" Switching from  " + this.GetType().Name + " to " + newState.GetType().Name);
        ExitState();
        newState.EnterState();
        if(_isRootState){
            _ctx.CurrentPlayerState = newState;

        }else{
            if(_currentSuperState != null){
                _currentSuperState.SetSubState(newState);
            }
        }
    }

    protected void SetSuperState(PlayerBaseState superState){
        _currentSuperState = superState;
    }

    protected  void SetSubState(PlayerBaseState subState){
        _currentSubState = subState;
        _currentSubState.SetSuperState(this);
        _currentSubState.EnterState();
    // Debug.Log($"Substate changed to: {_currentSubState.GetType().Name}");
    }


    
}
