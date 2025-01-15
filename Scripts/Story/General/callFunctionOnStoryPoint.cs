using UnityEngine;
using UnityEngine.Events;

public class callFunctionOnStoryPoint : MonoBehaviour, IStoryPointResponder
{
    public int self_order;
    public string self_name;
    public StoryPointInvoker storyPointInvoker; 

    public UnityEvent subscribe; 

    public void OnStoryPointEpisodeStart(int order)
    {
        if(order == self_order)
        {
            subscribe.Invoke();
        }
        Done(); 
    }

    public void Done()
    {
        storyPointInvoker.ResponderDone(self_order, self_name);
    }

    public int GetOrder()
    {
        return self_order;
    }

    public string GetName()
    {
        return self_name;
    }
}
