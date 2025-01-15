using UnityEngine;  

public class StoryPointWaiterSignalBased : MonoBehaviour , IStoryPointResponder{

    public int self_order;
    public string self_name;
    public int signalID; 
    public StoryPointInvoker storyPointInvokerObject; 

    public void OnSignal(int id){
        if(id == signalID){
            Done(); 
        }
    }
    //implement the interface
    public void OnStoryPointEpisodeStart(int order){
        Debug.Log("Story Point Waiter Signal : " + self_order.ToString() + " - " + self_name);
        UniSignals.Instance.ListenToSignal(OnSignal);
    }
    
    public void Done(){
        Debug.Log("Done --- Story Point Waiter Signal : " + self_order.ToString() + " - " + self_name);
        storyPointInvokerObject.ResponderDone(self_order, self_name);
        UniSignals.Instance.StopListeningToSignal(OnSignal);
    }
    public int GetOrder(){
        return self_order;
    }
    public string GetName(){
        return self_name;
    }

    
}