using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TimelineFadeOutPlayableAsset : PlayableAsset
{
    public ExposedReference<UIFadeController> target; // Reference to the UIFadeController component

    public bool isFadeOut = true; // If true, perform FadeOut; otherwise, perform FadeIn

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TimelineFadeOutPlayableBehaviour>.Create(graph);
        TimelineFadeOutPlayableBehaviour behaviour = playable.GetBehaviour();

        // Resolve the target reference
        behaviour.target = target.Resolve(graph.GetResolver());
        behaviour.isFadeOut = isFadeOut;

        return playable;
    }
}


public class TimelineFadeOutPlayableBehaviour : PlayableBehaviour
{
    public UIFadeController target; // Resolved target object
    public bool isFadeOut;          // If true, perform FadeOut; otherwise, FadeIn

    // Called when the clip starts playing
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (target != null)
        {
            double duration = playable.GetDuration(); // Duration of the clip
            if (isFadeOut)
            {
                target.FadeOut((float)duration); // Perform FadeOut
            }
            else
            {
                target.FadeIn((float)duration); // Perform FadeIn
            }
        }
        else
        {
            Debug.LogWarning("TimelineFadeOutPlayableBehaviour: Target is null!");
        }
    }
}
