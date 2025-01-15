using UnityEngine;
using System;
public class CollectibleInteractable : MonoBehaviour, IInteractable
{
    // Implements IInteractable
    public bool IsInteractable => true; // Always interactable for this simple case
    public Action OnAnimationComplete { get; set; } = null;
    public InteractionType GetInteractionType()
    {
        return InteractionType.Collectible; // This collectible uses the Default interaction type
    }

    public void Interact()
    {
        // Bare-bones: Log interaction
        Debug.Log($"{gameObject.name} was collected!");

        // Optionally destroy the object
        //Destroy(gameObject);
    }
}
