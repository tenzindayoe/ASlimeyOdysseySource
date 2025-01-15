using UnityEngine; 

public class PlayerRunState: PlayerBaseState{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){}

    private float timeElapsed = 0f;
    private float runSoundInterval = 0.34f;
    public override void EnterState(){
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsRunning, true);  
        _ctx.CharacterAnimator.SetBool(_ctx.HashIsWalking, false);
        _ctx.playerVFXManager.EnableWalkEffect();
        _ctx.playerVFXManager.playWalk(); 
        
        
    }
    public  override void UpdateState(){
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

        // Apply running speed multiplier
        float speed = _ctx.RunMultiplier;

        _ctx.AppliedMovementX = desiredMoveDirection.x * speed;
        _ctx.AppliedMovementZ = desiredMoveDirection.z * speed;
        //Sound Management
        timeElapsed += Time.deltaTime;
        if(timeElapsed >= runSoundInterval){
            GameAudioManager.Instance.PlayAudio(GameAudioManager.Instance.walkSound, GameAudioManager.Instance.envMixerGroup, true, _ctx.transform.position);
            timeElapsed = 0f;
        }

        CheckSwitchStates();
    }
    
    public override  void ExitState(){
        _ctx.playerVFXManager.disableWalkEffect();
        _ctx.playerVFXManager.StopWalk(); 
    }

    public override  void CheckSwitchStates(){
        if(!_ctx.IsRunPressed && _ctx.IsMovementPressed){
            SwitchState(_factory.Walk());
        }else if(!_ctx.IsMovementPressed){
            SwitchState(_factory.Idle());
        }
    }
    public override  void InitializeSubState(){}
}