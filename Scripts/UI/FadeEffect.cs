using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public Image fadeOutImage; // The image to fade in/out
    public float fadeInDuration = 1.5f;
    public float fadedStayDuration = 0.5f;
    public float fadeOutDuration = 1.5f;

    private Coroutine currentFadeCoroutine; // Track the active fade coroutine

    // Trigger a full fade sequence: FadeIn -> StayFaded -> FadeOut
    public void TriggerFullFade()
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);
        currentFadeCoroutine = StartCoroutine(FullFadeEffectRoutine());
    }

    // Trigger fade-in
    public void FadeIn()
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);
        currentFadeCoroutine = StartCoroutine(FadeAlpha(0f, 1f, fadeInDuration));
    }

    // Trigger fade-out
    public void FadeOut()
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);
        currentFadeCoroutine = StartCoroutine(FadeAlpha(1f, 0f, fadeOutDuration));
    }

    // Generalized alpha fade coroutine
    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color imageColor = fadeOutImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration); // Interpolate alpha
            imageColor.a = alpha; // Update alpha
            fadeOutImage.color = imageColor; // Apply color to the image
            yield return null;
        }

        imageColor.a = endAlpha; // Ensure final alpha value
        fadeOutImage.color = imageColor;
        currentFadeCoroutine = null; // Clear active coroutine reference
    }

    // Full fade effect: FadeIn -> StayFaded -> FadeOut
    private IEnumerator FullFadeEffectRoutine()
    {
        yield return StartCoroutine(FadeAlpha(0f, 1f, fadeInDuration)); // Fade in
        yield return new WaitForSeconds(fadedStayDuration); // Stay faded
        yield return StartCoroutine(FadeAlpha(1f, 0f, fadeOutDuration)); // Fade out
    }
}
