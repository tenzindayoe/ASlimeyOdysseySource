using System;
using UnityEngine; 


public enum ScaleType{
    Shrink, 
    Expand
}
public enum ScaleCheckType{
    AnyAxis, 
    AllAxis
}

public class ScaleResponder:MonoBehaviour{


    
    public Action atTriggerScale; 
    private Vector3 originalScale; 

    [Header("Scale Settings")]
    [Header("The scale that the object will be set to when the trigger is activated")]
    [Header("Note that the trigger scale is added to the original scale of the object")]
    [Header("This is done so that the object can be scaled up or down freely")]
    public Vector3 triggerScale;
    [Header("This defines whether we should check for being smaller or larger than the trigger scale")]
    public ScaleType scaleType = ScaleType.Expand;
    public ScaleCheckType scaleCheckType = ScaleCheckType.AllAxis;


    public void Start(){
        originalScale = this.transform.localScale; 
    }

    public void Update(){
        Vector3 limitScale = originalScale + triggerScale;  
        if(scaleType == ScaleType.Shrink){
            if(scaleCheckType == ScaleCheckType.AnyAxis){
                if(this.transform.localScale.x <= limitScale.x || this.transform.localScale.y <= limitScale.y || this.transform.localScale.z <= limitScale.z){
                    atTriggerScale?.Invoke(); 
                }
                
            }else if(scaleCheckType == ScaleCheckType.AllAxis){
                if(this.transform.localScale.x <= limitScale.x && this.transform.localScale.y <= limitScale.y && this.transform.localScale.z <= limitScale.z){
                    atTriggerScale?.Invoke();
                }
                
            }
        }else if(scaleType == ScaleType.Expand){
            if(scaleCheckType == ScaleCheckType.AnyAxis){
                if(this.transform.localScale.x >= limitScale.x || this.transform.localScale.y >= limitScale.y || this.transform.localScale.z >= limitScale.z){
                    atTriggerScale?.Invoke(); 
                }
                
            }else if(scaleCheckType == ScaleCheckType.AllAxis){
                if(this.transform.localScale.x >= limitScale.x && this.transform.localScale.y >= limitScale.y && this.transform.localScale.z >= limitScale.z){
                    atTriggerScale?.Invoke();
                }
            }
            
        }
    }


    // now i want you to do this, whenever there is  a scale change, i want you to call the jellymesh to shake


}