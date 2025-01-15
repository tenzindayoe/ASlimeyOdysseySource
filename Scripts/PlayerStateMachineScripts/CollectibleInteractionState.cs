using System;
using UnityEngine;

public class CollectibleInteractionState : PlayerBaseState
{
    private bool _isAnimationComplete = false;

    public CollectibleInteractionState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Entering Collectible Interaction State");

        // Trigger collectible interaction animation
        _ctx.CharacterAnimator.SetTrigger("Collect");
        _isAnimationComplete = false;
        _ctx.AnimEventManager.OnCollectingAnimationComplete += CompleteInteraction;
    }

    public override void UpdateState()
    {
        CheckSwitchStates(); // Should be called last
    }

    public override void ExitState()
    {
        _ctx.AnimEventManager.OnCollectingAnimationComplete -= CompleteInteraction;
        Debug.Log("Exiting Collectible Interaction State");
        _isAnimationComplete = false;
    }

    public override void CheckSwitchStates()
    {
        if (_isAnimationComplete)
        {
            PlayerInteractionState currentSuperState = _currentSuperState as PlayerInteractionState;
            currentSuperState.CurrentInteractionDone = true;
        }
    }

    public override void InitializeSubState()
    {
        // No substates needed for collectible interactions
    }

    private void CompleteInteraction()
    {
        Debug.Log("Collectible Interaction Complete");
        _isAnimationComplete = true;
        // Add logic for updating inventory or score
        var collectible = _ctx.GetInteractableInRange() as IInteractable;
        collectible?.Interact(); // Trigger the collectible's interaction logic
    }
}
