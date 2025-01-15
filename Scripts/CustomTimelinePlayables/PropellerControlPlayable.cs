using UnityEngine;
using UnityEngine.Playables;

public class PropellerControlPlayable : PlayableAsset
{
    public ExposedReference<PropellerRotation> target; // Reference to the PropellerRotation object
    public bool spin; // Determines whether the propeller should spin during this clip

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // Create a playable and get its behaviour
        var playable = ScriptPlayable<PropellerControlBehaviour>.Create(graph);
        PropellerControlBehaviour behaviour = playable.GetBehaviour();

        // Resolve the target object reference
        behaviour.target = target.Resolve(graph.GetResolver());
        behaviour.spin = spin; // Pass the spin state to the behaviour

        return playable;
    }
}

public class PropellerControlBehaviour : PlayableBehaviour
{
    public PropellerRotation target; // Resolved target object
    public bool spin; // Determines whether to start or stop spinning

    // Called when the clip starts playing
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (target != null)
        {
            if (spin)
            {
                target.startSpinning(); // Start spinning if spin is true
            }
            else
            {
                target.stopSpinning(); // Stop spinning if spin is false
            }
        }
    }
}
