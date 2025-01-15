
using System;
public enum InteractionType
{
    Collectible,
    CageBird
}

public interface IInteractable
{
    void Interact();
    bool IsInteractable { get; }
    InteractionType GetInteractionType();

    Action OnAnimationComplete { get; set; }
}
