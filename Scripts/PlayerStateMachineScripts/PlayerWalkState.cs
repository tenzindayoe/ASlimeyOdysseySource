using UnityEngine; 

public class PlayerWalkState: PlayerBaseState{

    private float timeElapsed = 0.55f; 
    private float walkSoundInterval = 0.55f;
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory){}


    public override void EnterState(){
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsWalking, true);
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsRunning, false);
        
    }


    public  override void UpdateState(){
        
        
        // Get camera-relative movement directions
        Vector3 camForward = _ctx.PlayerCamera.transform.forward;
        Vector3 camRight = _ctx.PlayerCamera.transform.right;
        
        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Calculate desired move direction
        Vector3 desiredMoveDirection =
            camForward * _ctx.CurrentMovementInputY +
            camRight * _ctx.CurrentMovementInputX;

        // Apply walking speed multiplier
        float speed = _ctx.WalkMultiplier;

        _ctx.AppliedMovementX = desiredMoveDirection.x * speed;
        _ctx.AppliedMovementZ = desiredMoveDirection.z * speed;

        //soundManagement 

        timeElapsed += Time.deltaTime;

        if(timeElapsed >= walkSoundInterval){
            //GameAudioManager.Instance.PlayAudio(GameAudioManager.Instance.walkSound, GameAudioManager.Instance.sfxMixerGroup, true, _ctx.transform.position);
            timeElapsed = 0f;
        }

        CheckSwitchStates();

    }
    
    public override  void ExitState(){
        
    }

    public override  void CheckSwitchStates(){

        if(_ctx.IsMovementPressed && _ctx.IsRunPressed){
            SwitchState(_factory.Run());
        }else if(!_ctx.IsMovementPressed){
            SwitchState(_factory.Idle());
        }   
    }
    public override  void InitializeSubState(){}
}