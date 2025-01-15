using UnityEngine;

public class PlayerInteractionState : PlayerBaseState
{
    private IInteractable _currentInteraction; // Reference to the current interaction object
    public IInteractable CurrentInteractionObject { get => _currentInteraction;}
    public bool CurrentInteractionDone;
    public PlayerInteractionState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
            _isRootState = true;
         }

    public override void EnterState()
    {
        Debug.Log("Entering PlayerInteractionState");
        CurrentInteractionDone = false; 
        _currentInteraction = _ctx.GetInteractableInRange(); // Cache the interactable object

        if (_currentInteraction == null)
        {
            Debug.LogWarning("From If 1 Player Interaction state: Current Interaction object is missing for some reason. ");
            SwitchState(_factory.Grounded());
        }
        else
        {
            InitializeSubState(); // Initialize the appropriate substate
        }
    }

    public override void UpdateState()
    {
        // Validate the current interaction object
        // if (_currentInteraction == null || !IsSameInteractionObject(_currentInteraction))
        // {
        //     Debug.LogWarning("From If 2, player interaction state. Interaction object is no longer valid or has changed.");
        //     SwitchState(_factory.Idle());
        //     return;
        // }

        // Check for transitions
        CheckSwitchStates(); // Should be called last
    }

    public override void ExitState()
    {
        Debug.Log("Exit :  PlayerInteractionState");
        _currentInteraction = null; // Clear the reference
    }

    public override void CheckSwitchStates()
    {
        if(CurrentInteractionDone){
            Debug.Log("The main switch case hit. ");
            SwitchState( _factory.Grounded());
        }
    }

    public override void InitializeSubState()
    {
        
        if (_currentInteraction != null)
        {
            switch (_currentInteraction.GetInteractionType())
            {
                // case InteractionType.Swing:
                //     SetSubState(_factory.SwingInteraction());
                //     break;
                // case InteractionType.OpenChest:
                //     SetSubState(_factory.OpenChestInteraction());
                //     break;
                // case InteractionType.ClimbLedger:
                //     SetSubState(_factory.ClimbLedgerInteraction());
                //     break;
                case InteractionType.CageBird:
                    var cageBirdState = _factory.CageBirdInteraction();
                    SetSubState(cageBirdState);
                    cageBirdState.EnterState();
                    break;
                default:
                    var collectibleState = _factory.CollectibleInteraction();
                    SetSubState(collectibleState);  // Set it as a substate
                    collectibleState.EnterState(); // Explicitly trigger EnterState
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Last if , the switch statement failed. No valid interactable found during InitializeSubState.");
            SwitchState(_factory.Idle());
        }
    }
    private bool IsSameInteractionObject(IInteractable interaction)
    {
        // Use a unique identifier like the GameObject instance ID to ensure it's the same object
        var interactableInRange = _ctx.GetInteractableInRange();
        return interactableInRange != null && interactableInRange == interaction;
    }
}
