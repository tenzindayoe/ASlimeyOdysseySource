using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialoguePlayableAsset : PlayableAsset
{

    
    public string dialogueText;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialoguePlayableBehaviour>.Create(graph);
        DialoguePlayableBehaviour behaviour = playable.GetBehaviour();
        behaviour.dialogueText = dialogueText;
        return playable;
    }
}
