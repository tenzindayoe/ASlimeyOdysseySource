
using UnityEngine; 
public class CustomEnemyStoryPoint : MonoBehaviour, IStoryPointResponder
{
    public int self_order;
    public string self_name;
    public StoryPointInvoker storyPointInvokerObject;

    public Animator animator; 
    public GameObject meshObj; 

    public GameObject effect1; 
    public GameObject effect2; 


    public void OnStoryPointEpisodeStart(int order)
    {
        if (order == self_order)
        {
            Debug.Log("SetParent : " + self_order.ToString() + " - " + self_name);
            SetParent();
        }
    }

    public void SetParent()
    {
        animator.enabled = false; 
        //disable mesh object 
        meshObj.SetActive(false);

        //enable effects
        effect1.SetActive(true);
        effect2.SetActive(true);
        
        Done();
    }

    public void Done()
    {
       
        storyPointInvokerObject.ResponderDone(self_order, self_name);
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