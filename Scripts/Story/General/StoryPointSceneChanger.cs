using UnityEngine; 

public class StoryPointSceneChanger: MonoBehaviour, IStoryPointResponder{
    public string sceneName;
    public SceneSwitcher sceneSwitcher; 
    public int self_order; 
    public string self_name;

    public StoryPointInvoker storyPointInvokerObject;

    public void OnStoryPointEpisodeStart(int order){
        if(order == self_order){
            
            ChangeScene();
            Done();
        }
    }

    private void ChangeScene(){
        sceneSwitcher.SwitchScene(sceneName, "Your journey has just began, exploration is the key!");
    }

    public void Done(){
        Debug.Log("Done --- Scene Change : " + self_order.ToString() + " - " + self_name);
        storyPointInvokerObject.ResponderDone(self_order, self_name);
    }

    public int GetOrder(){
        return self_order;
    }

    public string GetName(){
        return self_name;
    }

}