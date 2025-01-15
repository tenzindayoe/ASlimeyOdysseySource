using UnityEngine;

public static class AnimationUtils
{
    /// <summary>
    /// Checks if the given Animator is currently playing any animation.
    /// </summary>
    public static bool IsPlayingAnAnimation(Animator animator)
    {
        if (animator == null) return false;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Handle looping animations by using modulus operator
        return stateInfo.length > 0 && (stateInfo.normalizedTime % 1) < 1;
    }

    /// <summary>
    /// Checks if the given Animator is currently playing the specified animation state.
    /// </summary>
    public static bool IsPlaying(Animator animator, string stateName)
    {
        if (animator == null) return false;

        int stateHash = Animator.StringToHash(stateName);
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        var nextStateInfo = animator.GetNextAnimatorStateInfo(0);
        var transitionInfo = animator.GetAnimatorTransitionInfo(0);

        // Check if the Animator is in a transition
        bool isInTransition = transitionInfo.fullPathHash != 0;

        // Check if the target state is either the current state or the next state during a transition
        bool isInCurrentState = stateInfo.shortNameHash == stateHash;
        bool isInNextState = isInTransition && nextStateInfo.shortNameHash == stateHash;

        return IsPlayingAnAnimation(animator) && (isInCurrentState || isInNextState);
    }

    /// <summary>
    /// Logs the current animation state for debugging purposes.
    /// </summary>
    public static void LogCurrentState(Animator animator)
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator is null!");
            return;
        }

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        var transitionInfo = animator.GetAnimatorTransitionInfo(0);
        var nextStateInfo = animator.GetNextAnimatorStateInfo(0);

        Debug.Log($"Current State: {stateInfo.shortNameHash}, " +
                  $"Next State: {nextStateInfo.shortNameHash}, " +
                  $"Is Transitioning: {transitionInfo.fullPathHash != 0}, " +
                  $"Is Playing: {IsPlayingAnAnimation(animator)}, " +
                  $"Normalized Time: {stateInfo.normalizedTime}");
    }
}
