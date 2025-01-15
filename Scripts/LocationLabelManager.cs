using UnityEngine;
using TMPro;  // Required for TextMeshPro

public class LocationLabelManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI locationLabel;  // Assign your TextMeshProUGUI component here

    private void Start()
    {
        if (UserCoordinates.Instance != null)
        {
            // Subscribe to place name updates
            UserCoordinates.Instance.PlaceNameUpdated += OnPlaceNameUpdated;
        }

        // Initialize the label
        UpdateLocationLabel("Unknown");
    }

    private void OnDestroy()
    {
        if (UserCoordinates.Instance != null)
        {
            // Unsubscribe from place name updates
            UserCoordinates.Instance.PlaceNameUpdated -= OnPlaceNameUpdated;
        }
    }

    private void OnPlaceNameUpdated(string newPlaceName)
    {
        UpdateLocationLabel(newPlaceName);
    }

    private void UpdateLocationLabel(string placeName)
    {
        if (placeName == "Unknown" || string.IsNullOrEmpty(placeName))
        {
            locationLabel.gameObject.SetActive(false);  // Hide the label
        }
        else
        {
            locationLabel.gameObject.SetActive(true);  // Show the label
            locationLabel.text = placeName;  // Update the label text
        }
    }
}
