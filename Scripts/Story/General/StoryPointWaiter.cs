using UnityEngine;
using System.Collections; 
public class StoryPointWaiter : MonoBehaviour, IStoryPointResponder{

    public int self_order;
    public string self_name;

    public float secondsToOccupy = 0f; 
    public StoryPointInvoker storyPointInvokerObject; 
    //implement the interface
    public void OnStoryPointEpisodeStart(int order){
        if(order == self_order){
            // do something
            Debug.Log("Story Point Waiter : " + self_order.ToString() + " - " + self_name);
            StartCoroutine(Occupy());
        }
        
    }
    public IEnumerator Occupy(){
        yield return new WaitForSeconds(secondsToOccupy);
        Done();
    }   
    public void Done(){
        Debug.Log("Done --- Story Point Waiter : " + self_order.ToString() + " - " + self_name);
        storyPointInvokerObject.ResponderDone(self_order, self_name);
    }
    public int GetOrder(){
        return self_order;
    }
    public string GetName(){
        return self_name;
    }
}