using UnityEngine;

public class PlaneLight : MonoBehaviour
{
    [Header("Light Settings")]
    public Light planeLight; // Reference to the light component
    public Material planeMaterial; // Reference to the material
    public Color lightColor = Color.red; // The color when the light is shining
    public Color baseColor = Color.white; // The color when the light is not shining
    public float maxLightIntensity = 5f; // Maximum intensity of the light
    public float period = 2f; // Total time for one full blend (shining to not shining and back)

    private float timer; // Tracks the time for blending

    private void Update()
    {
        if (planeLight == null || planeMaterial == null)
        {
            Debug.LogError("Please assign a Light and Material to the PlaneLight script.");
            return;
        }

        // Update timer
        timer += Time.deltaTime;
        float blendFactor = Mathf.PingPong(timer / period, 1f);

        // Update light intensity
        planeLight.intensity = Mathf.Lerp(0f, maxLightIntensity, blendFactor);

        // Update material color
        Color currentColor = Color.Lerp(baseColor, lightColor, blendFactor);
        planeMaterial.SetColor("_EmissionColor", currentColor * Mathf.LinearToGammaSpace(planeLight.intensity));
    }
}
