using UnityEngine;
using System;
public class CageBirdInteractable: MonoBehaviour, IInteractable{
    public bool IsInteractable => true;
    public Action OnAnimationComplete { get; set; } = ()=> {};

    public Animator animator; 
    public void Interact(){
        Debug.Log("CageBirdInteractable.Interact()");   
        animator.SetTrigger("Open");
    }
    public InteractionType GetInteractionType()
    {
        return InteractionType.CageBird; // This collectible uses the Default interaction type
    }
    public void OnOpen(){
        Debug.Log("CageBirdInteractable.OnChestOpen()");    
        OnAnimationComplete?.Invoke();
    }

}