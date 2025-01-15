using UnityEngine; 


public class ChangeInteractableStateSimpleSPOnStoryPoint : MonoBehaviour, IStoryPointResponder{

    public int self_order; 
    public string self_name;

    public StoryPointInvoker storyPointInvokerObject;

    public GameObject interactableGameObject;
    public bool isActive; 

    public void OnStoryPointEpisodeStart(int order){
        if(order == self_order){
            Debug.Log("Change Interactable State : " + self_order.ToString() + " - " + self_name);
            ChangeState();
            Done();
        }
    }

    private void ChangeState(){
        interactableGameObject.GetComponent<SimpleSPInteractable>().isInteractable = isActive; 
    }

    public void Done(){
        Debug.Log("Done --- Change Interactable State : " + self_order.ToString() + " - " + self_name);
        storyPointInvokerObject.ResponderDone(self_order, self_name);
    }

    public int GetOrder(){
        return self_order;
    }

    public string GetName(){
        return self_name;
    }
}