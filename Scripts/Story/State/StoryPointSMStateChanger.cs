using UnityEngine; 

public class StoryPointSMStateChanger:MonoBehaviour, IStoryPointResponder{

    public int self_order;
    public string self_name;
    

    public BaseStartState targetState; 
    public PlayerStateMachine playerStateMachine;
    public StoryPointInvoker storyPointInvokerObject; 
    [Header("Signal ID for Interactive Cutscene Only")]
    public int signalID;
    //implement the interface
    public void OnStoryPointEpisodeStart(int order){
        Debug.Log("State machine changer : " + self_order.ToString() + " - " + self_name);
        if(order == self_order){
           if (playerStateMachine != null){
               switch(targetState){
                case BaseStartState.InteractiveCutscene:
                    playerStateMachine.SetInteractiveCutsceneAsCurrentState(signalID);
                    break;
                case BaseStartState.Cutscene:
                    playerStateMachine.SetCutsceneAsCurrentState();
                    break;
                case BaseStartState.Aircraft:
                    playerStateMachine.SetAircraftAsCurrentState();
                    break;
                case BaseStartState.Grounded:
                    playerStateMachine.SetGroundedAsCurrentState();
                    break;
                default:
                    break; 
               }
           }
        }
        Done(); 
        
    }

    public void Done(){
        Debug.Log("Done --- State machine changer : " + self_order.ToString() + " - " + self_name);
        storyPointInvokerObject.ResponderDone(self_order, self_name);
    }
    public int GetOrder(){
        return self_order;
    }
    public string GetName(){
        return self_name;
    }
    
}