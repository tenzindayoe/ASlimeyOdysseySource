using UnityEngine;
using UnityEngine.Playables;

public class FadePlayableBehaviour : PlayableBehaviour
{
    public FadeEffect target;  // Resolved target object
    public bool triggerFadeOut; // Determines whether to call FadeIn or FadeOut

    // Called when the clip starts playing
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (target != null)
        {
            if (triggerFadeOut)
            {
                target.FadeOut(); // Call wrapper for FadeOut
            }
            else
            {
                target.TriggerFullFade(); // Call wrapper for FullFadeEffectRoutine
            }
        }
        else
        {
            Debug.LogWarning("FadePlayableBehaviour: Target is null!");
        }
    }
}


[System.Serializable]
public class FadePlayableAsset : PlayableAsset
{
    public ExposedReference<FadeEffect> target; // Reference to the FadeEffect component
    public bool triggerFadeOut;                // Whether to fade out or fade in

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // Create a playable and get its behaviour
        var playable = ScriptPlayable<FadePlayableBehaviour>.Create(graph);
        FadePlayableBehaviour behaviour = playable.GetBehaviour();

        // Resolve the target object reference
        behaviour.target = target.Resolve(graph.GetResolver());
        behaviour.triggerFadeOut = triggerFadeOut; // Pass the fade state to the behaviour

        return playable;
    }
}
