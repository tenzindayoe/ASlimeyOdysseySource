using UnityEngine; 

public class SimpleSPInteractable:MonoBehaviour, IStoryPointInteractable{

    public int signalID;
    public bool isInteractable = true;

    public bool deactivateAfterInteraction = false;
    public bool disableAfterInteraction = true; 
    public void Interact(){
       
        if (!isInteractable){
            return;
        }
        if(disableAfterInteraction){
            isInteractable = false;
        }
        // call the send singla from IStoryPointInteractable
        SendSignal();
        if(deactivateAfterInteraction){
            this.gameObject.SetActive(false);
        }

    }

    public void SetSignalID(int id){
        signalID = id;
    }

    public bool IsInteractable(){
        return isInteractable;
    }
    public int GetSignalID(){
        return signalID;
    }

    public void SendSignal(){
        UniSignals.Instance.EmitSignal(GetSignalID());
    }


}