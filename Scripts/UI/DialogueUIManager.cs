using UnityEngine; 
using TMPro; 
public class DialogueUIManager : MonoBehaviour{


    public static DialogueUIManager Instance;
    public TextMeshProUGUI dialogueText;
    void Awake(){
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(this.gameObject); // Destroy duplicate instances
        }
    }
    
    public void ShowDialogue(string dialogueText){
        //Debug.Log("DialogueUIManager: ShowDialogue: " + dialogueText);
        this.dialogueText.text = dialogueText;

    }

    public void ShowDialogueTimed(string dialogueText, float time){
        ShowDialogue(dialogueText);
        Invoke("HideDialogue", time);
    }
    public void HideDialogue(){
        dialogueText.text = "";
    }
}