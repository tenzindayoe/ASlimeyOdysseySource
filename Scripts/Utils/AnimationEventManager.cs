using System;
using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    // Define events for animation callbacks
    public event Action OnCollectingAnimationComplete;


    public void OnCollectingAnimDone()
    {
        Debug.Log("AnimationEventManager: Collecting animation done");
        OnCollectingAnimationComplete?.Invoke(); // Trigger subscribed handlers
        
    
        
    }

}
