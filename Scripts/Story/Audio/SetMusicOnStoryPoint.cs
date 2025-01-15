using UnityEngine; 


public class SetMusicOnStoryPoint: MonoBehaviour, IStoryPointResponder{

    public int self_order;
    public string self_name; 
    public StoryPointInvoker storyPointInvokerObject; 
    
    public MusicTracks currentMusicTrack;
    
    //implement the interface
    public void OnStoryPointEpisodeStart(int order){
        
        if(order == self_order){
            // do something
            Debug.Log("Music : " + self_order.ToString() + " - " + self_name);
            MusicManager.Instance.PlayMusic(currentMusicTrack);
            Done();
        }
        
    }
    public void Done(){
        Debug.Log("Done --- Music : " + self_order.ToString() + " - " + self_name);
        storyPointInvokerObject.ResponderDone(self_order, self_name); 
    }
    public int GetOrder(){
        return self_order;
    }

    public string GetName(){
        return self_name;
    }
}