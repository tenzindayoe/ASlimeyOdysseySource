using UnityEngine;
using UnityEngine.Playables;

public class DialoguePlayableBehaviour : PlayableBehaviour
{
    public string dialogueText;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!Application.isPlaying) return;

        // Show the dialogue
        DialogueUIManager.Instance?.ShowDialogue(dialogueText);
    }
}
