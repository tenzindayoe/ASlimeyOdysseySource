using UnityEngine;
using TMPro; // Ensure TextMeshPro is imported
using CesiumForUnity;

public class TileManager : MonoBehaviour
{
    public Cesium3DTileset _tileset;
    public TextMeshProUGUI loadingText; // Assign your TMP text object in the Unity Inspector
    private bool isFullyLoaded = false;

    private void Start()
    {

        if (_tileset == null)
        {
            Debug.LogError("Cesium3DTileset component is missing on this GameObject!");
        }
    }

    private void Update()
    {
        if (_tileset == null || loadingText == null) return;

        // Compute the loading progress (0.0 to 100.0)
        float loadProgress = _tileset.ComputeLoadProgress() * 100.0f; // Scale to percentage

        if (!isFullyLoaded)
        {
            loadingText.text = $"Loading Progress: {loadProgress:0.00}%";

            // Check if the tileset is fully loaded
            if (loadProgress >= 100.0f)
            {
                isFullyLoaded = true;
                OnTileLoadComplete();
            }
        }
    }

    private void OnTileLoadComplete()
    {
        // Update the TMP text when loading is complete
        loadingText.text = "Tile Loading Complete!";
    }
}
