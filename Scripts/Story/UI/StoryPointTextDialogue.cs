using UnityEngine;

public class StoryPointTextDialogue : MonoBehaviour, IStoryPointResponder
{
    public int self_order; 
    public string self_name;
    public StoryPointInvoker storyPointInvokerObject; 

    public float duration;

    public AudioClip dialogueClip; 

    public bool useAudioClipLength = false;  
    public bool addAuthorName = true; 
    public string dialogue;
    public string speaker;

    
    public void OnStoryPointEpisodeStart(int order)
    {
        if (order == self_order)
        {
            string text; 
            if(addAuthorName){
                text = speaker + " : " + dialogue;
            }else{
                text = dialogue;
            }

            if(useAudioClipLength){
                if(dialogueClip == null){
                    Debug.LogError("No audio clip found for dialogue");
                    return;
                }
                DialogueUIManager.Instance.ShowDialogueTimed(text, dialogueClip.length);
            }else{
                DialogueUIManager.Instance.ShowDialogueTimed( text, duration);

            }
            Invoke("Done", duration);
        }
    }

    public void Done()
    {
        // Implement your logic here
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
