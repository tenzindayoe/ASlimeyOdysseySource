using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using CesiumForUnity;

public class UserCoordinates : MonoBehaviour
{
    public static UserCoordinates Instance { get; private set; }  // Singleton instance

    [Header("Dependencies")]
    public Transform planeTransform;  // Assign your plane's transform
    public CesiumGeoreference georeference;  // Assign the CesiumGeoreference component

    [Header("Settings")]
    public float updateInterval = 3.0f;  // Time in seconds between API calls
    public float minCoordinateChange = 0.001f;  // Minimum change in lat/lon to trigger API call

    private string lastPlace = "Unknown";
    private double lastLatitude, lastLongitude;

    public delegate void OnPlaceNameUpdated(string placeName);
    public event OnPlaceNameUpdated PlaceNameUpdated;  // Event for place name updates

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // Optional: persist this object across scenes
    }

    private void Start()
    {
        StartCoroutine(ReverseGeocodingRoutine());
    }

    private IEnumerator ReverseGeocodingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            GetPlaceName();
        }
    }

    private void GetPlaceName()
    {
        if (!planeTransform || !georeference) return;

        // Get the plane's Unity position
        Vector3 unityPosition = planeTransform.position;

        // Convert Unity position to ECEF coordinates
        var unityPositionDouble3 = new Unity.Mathematics.double3(unityPosition.x, unityPosition.y, unityPosition.z);
        var ecef = georeference.TransformUnityPositionToEarthCenteredEarthFixed(unityPositionDouble3);

        // Convert ECEF to latitude and longitude
        var lonLatHeight = CesiumForUnity.CesiumWgs84Ellipsoid.EarthCenteredEarthFixedToLongitudeLatitudeHeight(ecef);
        double latitude = lonLatHeight.y;
        double longitude = lonLatHeight.x;

        // Call the API only if the coordinates have changed
        if (Mathf.Abs((float)(latitude - lastLatitude)) > minCoordinateChange || Mathf.Abs((float)(longitude - lastLongitude)) > minCoordinateChange)
        {
            lastLatitude = latitude;
            lastLongitude = longitude;
            StartCoroutine(FetchPlaceNameFromAPI(latitude, longitude));
        }
    }

    private IEnumerator FetchPlaceNameFromAPI(double latitude, double longitude)
    {
        string url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={latitude}&lon={longitude}&zoom=10&addressdetails=1";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("User-Agent", "UnityReverseGeocodingApp");  // Required for Nominatim API
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                ProcessAPIResponse(response);
            }
            else
            {
                Debug.LogError($"API Error: {request.error}");
            }
        }
    }

    private void ProcessAPIResponse(string response)
    {
        var jsonResponse = JsonUtility.FromJson<NominatimResponse>(response);

        if (!string.IsNullOrEmpty(jsonResponse.display_name))
        {
            Debug.Log($"Place Name: {jsonResponse.display_name}");
            lastPlace = jsonResponse.display_name;

            // Trigger the event
            PlaceNameUpdated?.Invoke(lastPlace);
        }
        else
        {
            Debug.LogWarning("No place name found in the response.");
        }
    }

    public string GetLastPlaceName()
    {
        return lastPlace;
    }

    public double[] GetLastCoordinates()
    {
        return new double[] { lastLatitude, lastLongitude };
    }

    // Define the response structure based on Nominatim's JSON format
    [System.Serializable]
    public class NominatimResponse
    {
        public string display_name;
    }
}
