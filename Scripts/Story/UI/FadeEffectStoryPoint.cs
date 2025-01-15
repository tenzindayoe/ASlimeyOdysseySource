using UnityEngine;

public class FadeEffectStoryPoint : MonoBehaviour, IStoryPointResponder
{
    public int self_order; // The order of this story point
    public string self_name; // The name of this story point
    public StoryPointInvoker storyPointInvokerObject; // Reference to the StoryPointInvoker
    public UIFadeController fadeController; // Reference to the UIFadeController

    public bool FadeOut = false; // If true, perform FadeOut; otherwise, FadeIn
    public float fadeDuration = 1.5f; // Duration of the fade effect, configurable in the Inspector

    public void OnStoryPointEpisodeStart(int order)
    {
        if (order == self_order)
        {
            if (FadeOut)
            {
                InitiateFadeOutOnly(); // Immediately start fade-out
            }
            else
            {
                InitiateFadeIn(); // Immediately start fade-in
            }

            Done(); // Call Done immediately
        }
    }

    public void InitiateFadeIn()
    {
        // Trigger the fade-in without waiting
        fadeController.FadeIn(fadeDuration);
    }

    public void InitiateFadeOutOnly()
    {
        // Trigger the fade-out without waiting
        fadeController.FadeOut(fadeDuration);
    }

    public void Done()
    {
        // Notify the StoryPointInvoker that this story point is complete
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
