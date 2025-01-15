using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PlayAudioOnStoryPoint: MonoBehaviour, IStoryPointResponder{

    public int self_order;
    public string self_name; 
    public StoryPointInvoker storyPointInvokerObject; 

    public AudioClip audioClip;

   
    //implement the interface
    public void OnStoryPointEpisodeStart(int order){
        if(order == self_order){
            // do something
            Debug.Log("Audio : " + self_order.ToString() + " - " + self_name);
            StartCoroutine(waitForAudio());
            
        }
        
    }
    public IEnumerator waitForAudio(){
        GameAudioManager.Instance.PlayAudio(audioClip, GameAudioManager.Instance.sfxMixerGroup, false, null);   
        yield return new WaitForSeconds(audioClip.length);
        Done();
    }
    public void Done(){
        Debug.Log("Done --- Audio : " + self_order.ToString() + " - " + self_name);
        storyPointInvokerObject.ResponderDone(self_order, self_name); 
    }
    public int GetOrder(){
        return self_order;
    }

    public string GetName(){
        return self_name;
    }

}