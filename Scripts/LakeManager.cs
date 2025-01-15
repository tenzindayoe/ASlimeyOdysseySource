using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;
using UnityEngine.Networking;
using Unity.Mathematics;

public class LakeManager : MonoBehaviour
{
    [Header("Dependencies")]
    public GameObject markerPrefab;
    public CesiumGeoreference cesiumGeoreference;
    public Transform userTransform;

    [Header("Settings")]
    public float updateInterval = 5.0f; // Time in seconds between updates
    public float lakeThresholdKm = 100.0f; // Search radius for lakes in kilometers

    private Dictionary<string, GameObject> activeMarkers = new Dictionary<string, GameObject>();

    private void Start()
    {
        StartCoroutine(UpdateLakeMarkersRoutine());
    }

    private IEnumerator UpdateLakeMarkersRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            UpdateLakeMarkers();
        }
    }

    private void UpdateLakeMarkers()
    {
        if (!cesiumGeoreference || !userTransform)
        {
            Debug.LogError("CesiumGeoreference or UserTransform is not assigned.");
            return;
        }

        // Get user coordinates
        var unityPosition = userTransform.position;
        var unityPositionDouble3 = new double3(unityPosition.x, unityPosition.y, unityPosition.z);
        var ecef = cesiumGeoreference.TransformUnityPositionToEarthCenteredEarthFixed(unityPositionDouble3);
        var lonLatHeight = CesiumWgs84Ellipsoid.EarthCenteredEarthFixedToLongitudeLatitudeHeight(ecef);
        double userLatitude = lonLatHeight.y;
        double userLongitude = lonLatHeight.x;

        Debug.Log($"User Position - Latitude: {userLatitude}, Longitude: {userLongitude}");

        // Query the lake search API
        StartCoroutine(QueryLakes(userLatitude, userLongitude, lakeThresholdKm));
    }

    private IEnumerator QueryLakes(double latitude, double longitude, float thresholdKm)
    {
        string url = $"http://localhost:8000/search_lakes?latitude={latitude}&longitude={longitude}&threshold_km={thresholdKm}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            Debug.Log($"Querying lakes within {thresholdKm} km of ({latitude}, {longitude})...");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log($"API Response: {response}");
                ProcessLakesResponse(response);
            }
            else
            {
                Debug.LogError($"Lake API Request Failed: {request.error}");
            }
        }
    }

    private void ProcessLakesResponse(string response)
    {
        try
        {
            // Wrap the response in a JSON object to allow parsing as a list
            string wrappedResponse = "{\"lakes\":" + response + "}";

            // Deserialize the response
            LakeList lakeList = JsonUtility.FromJson<LakeList>(wrappedResponse);

            if (lakeList == null || lakeList.lakes == null || lakeList.lakes.Count == 0)
            {
                Debug.LogWarning("No lakes found in the current search.");
                return;
            }

            Debug.Log($"Found {lakeList.lakes.Count} lakes.");

            // Track active markers to manage additions and removals
            HashSet<string> lakesInScope = new HashSet<string>();

            foreach (var lake in lakeList.lakes)
            {
                lakesInScope.Add(lake.name);

                if (!activeMarkers.ContainsKey(lake.name))
                {
                    // Spawn a new marker if it's not already active
                    SpawnLakeMarker(lake);
                }
            }

            // Remove markers that are no longer in scope
            List<string> markersToRemove = new List<string>();
            foreach (var activeMarker in activeMarkers)
            {
                if (!lakesInScope.Contains(activeMarker.Key))
                {
                    markersToRemove.Add(activeMarker.Key);
                }
            }

            foreach (var markerName in markersToRemove)
            {
                Destroy(activeMarkers[markerName]);
                activeMarkers.Remove(markerName);
                Debug.Log($"Removed marker for lake: {markerName}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error processing lake response: {ex.Message}");
        }
    }

    private void SpawnLakeMarker(Lake lake)
    {
        if (markerPrefab == null || cesiumGeoreference == null)
        {
            Debug.LogError("MarkerPrefab or CesiumGeoreference is not assigned.");
            return;
        }

        // Instantiate the marker prefab
        GameObject marker = Instantiate(markerPrefab);
        marker.name = lake.name;

        // Now use our dedicated function to place the marker
        PlaceMarker(marker, lake.latitude, lake.longitude, 100);

        // Store the marker in the activeMarkers dictionary
        activeMarkers[lake.name] = marker;
    }

    private void PlaceMarker(GameObject marker, double latitude, double longitude, double height)
    {
        // Ensure the CesiumGlobeAnchor is attached
        marker.transform.SetParent(cesiumGeoreference.transform, false);
        CesiumGlobeAnchor globeAnchor = marker.GetComponent<CesiumGlobeAnchor>();
        if (globeAnchor == null)
        {
            globeAnchor = marker.AddComponent<CesiumGlobeAnchor>();
        }

        // Assign the geospatial position
        globeAnchor.longitudeLatitudeHeight = new double3(longitude, latitude, height);
        // Log placement details for debugging
        Debug.Log($"Placed marker at Lat: {latitude}, Lon: {longitude}, Height: {height}m");
    }

    [System.Serializable]
    public class Lake
    {
        public string name;
        public double latitude;
        public double longitude;
        public double distance_km;
    }

    [System.Serializable]
    public class LakeList
    {
        public List<Lake> lakes;
    }
}
