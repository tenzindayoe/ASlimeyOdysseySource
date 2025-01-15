using System;
using System.Collections.Generic;
using UnityEngine; 

[Serializable]
public struct ActivationObject {
    public GameObject gameObject;
    public bool isActive;
}



public class SetActivationsOnStoryPoint: MonoBehaviour, IStoryPointResponder{

    public List<ActivationObject> activationObjects = new List<ActivationObject>();

    public int self_order; 
    public string self_name;

    public StoryPointInvoker storyPointInvokerObject;

    public void OnStoryPointEpisodeStart(int order){
        if(order == self_order){
            Debug.Log("Set Activations : " + self_order.ToString() + " - " + self_name);
            SetActivations();
            Done();
        }
    }

    private void SetActivations(){
        foreach(ActivationObject activationObject in activationObjects){
            activationObject.gameObject.SetActive(activationObject.isActive);
            Debug.Log("Set Activation : " + activationObject.gameObject.name + " - " + activationObject.isActive);
        }
        
    }

    public void Done(){
        Debug.Log("Done --- Set Activations : " + self_order.ToString() + " - " + self_name);
        storyPointInvokerObject.ResponderDone(self_order, self_name);
    }

    public int GetOrder(){
        return self_order;
    }

    public string GetName(){
        return self_name;
    }

}