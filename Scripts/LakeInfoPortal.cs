using DG.Tweening;
using UnityEngine;

public class LakeInfoPortal : MonoBehaviour
{
    public bool IsSelected = false; // Tracks if this portal is selected
    public Vector3 animationScale = new Vector3(0.005f, 0.005f, 0.005f); 
    public Vector3 animationShake = new Vector3(0.005f, 0.005f, 0.005f); 
    public DOTweenAnimation selfTween; 

    void Start(){
        selfTween = GetComponent<DOTweenAnimation>(); 
    }
    // Called when the portal is selected
    public void OnFocusEnter()
    {
        if (!IsSelected)
        {
            Debug.Log($"{gameObject.name} is now focused.");
            PlayScaleShakeTween(animationScale, 2);
            // Add visual feedback (e.g., highlight)
        }
    }

    // Called when the portal loses focus
    public void OnFocusExit()
    {
        if (!IsSelected)
        {
            Debug.Log($"{gameObject.name} focus removed.");
            
            PlayScaleShakeTween(animationScale, 1);
            // Remove visual feedback
        }
    }

    // Called when the portal is left-clicked
    public void OnLeftClick()
    {
        if (!IsSelected)
        {
            IsSelected = true;
            Debug.Log($"{gameObject.name} is selected.");
            PlayRotationShakeTween(2);
            
            // Perform any action, such as displaying detailed lake info
        }
    }

    // Called to unselect the portal
    public void unselect()
    {
        if (IsSelected)
        {
            IsSelected = false;
            Debug.Log($"{gameObject.name} is unselected.");
            // Reset any visual feedback or UI updates
        }
    }

     private void PlayScaleShakeTween(Vector3 shake, float duration)
    {
        transform.DOShakeScale(duration, shake, vibrato: 10, randomness: 90).SetEase(Ease.OutQuad);
        
    }

    // Tween for rotation shaking
    private void PlayRotationShakeTween(float duration)
    {
        transform.DOShakeRotation(duration, strength: new Vector3(10f, 10f, 10f), vibrato: 10, randomness: 90)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Debug.Log("Rotation shake completed."));
    }
      
    
}
