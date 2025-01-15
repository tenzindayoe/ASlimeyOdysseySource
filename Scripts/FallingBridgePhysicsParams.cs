using UnityEngine;

public class FallingBridgePhysicsParams : MonoBehaviour{


    public float maxBreakTorque ; 
    public float maxBreakForce ;

    public FixedJoint toBreakJoint; 

    // should be called from the Falling Bride Constructor
    public void Setup(){
        toBreakJoint.breakForce = maxBreakForce;
        toBreakJoint.breakTorque = maxBreakTorque;
    
    }
}