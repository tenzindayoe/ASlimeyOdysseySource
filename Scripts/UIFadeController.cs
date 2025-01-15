using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeController : MonoBehaviour
{
    public Image fadeImage; // Black image to simulate fade effect
    private Coroutine currentFadeCoroutine; // Track the active fade coroutine

    // Fade out: Screen transitions to black (alpha = 1)
    public void FadeOut(float duration)
    {

        
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine); // Stop any ongoing fade
        currentFadeCoroutine = StartCoroutine(FadeAlpha(fadeImage.color.a, 1f, duration)); // Start new fade
    }

    // Fade in: Screen transitions to transparent (alpha = 0)
    public void FadeIn(float duration)
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine); // Stop any ongoing fade
        currentFadeCoroutine = StartCoroutine(FadeAlpha(fadeImage.color.a, 0f, duration)); // Start new fade
    }

    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        
        float elapsedTime = 0f;

        // Get the current color of the image
        Color color = fadeImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration); // Interpolate alpha
            color.a = alpha; // Update the alpha value
            fadeImage.color = color; // Apply the updated color
            yield return null;
        }

        // Ensure the final alpha value is set
        color.a = endAlpha; // Update the alpha value
        fadeImage.color = color; // Apply the final color

        currentFadeCoroutine = null; // Clear the reference to the coroutine
    }
}
