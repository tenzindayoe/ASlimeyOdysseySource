// PlayTimelineOnStoryPoint.cs
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Playables;

public class PlayTimelineOnStoryPoint : MonoBehaviour, IStoryPointResponder
{
    //requires a timeline player attached to the same object. the bindings are set in the playable director
    public int self_order;
    public string self_name; 
    public StoryPointInvoker storyPointInvokerObject; 

    //get the timeline player
    public PlayableDirector timelinePlayer;
    void Start(){
        if(timelinePlayer == null)
        timelinePlayer = GetComponent<PlayableDirector>();
    }

    //implement the interface
    public void OnStoryPointEpisodeStart(int order){
        if(order == self_order){
            Debug.Log("Timeline : " + self_order.ToString() + " - " + self_name);
            PlayTimeline();
        }
        
    }
    public void PlayTimeline(){
        timelinePlayer.Play();
        StartCoroutine(WaitForTimeline());

    }
    public IEnumerator WaitForTimeline(){
        yield return new WaitForSeconds((float)timelinePlayer.duration);
        Done();
    }

    
    public void Done(){
        Debug.Log("Done --- Timeline : " + self_order.ToString() + " - " + self_name);
        storyPointInvokerObject.ResponderDone(self_order, self_name);
    }
    public int GetOrder(){
        return self_order;
    }
    public string GetName(){
        return self_name;
    }

}
