using System;
using System.Collections;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(1000)] // Ensures this script runs after CesiumGeoreference
public class TravelSceneLoader : MonoBehaviour
{   
    [SerializeField]
    private CesiumGeoreference _cesiumGeoreference;
    [SerializeField]
    private CesiumGlobeAnchor _playerGlobalAnchor;  
    [SerializeField]
    private CesiumGlobeAnchor _originShifter; 
    [SerializeField]
    private CesiumGlobeAnchor _cameraGlobalAnchor;


    
    void OnEnable()
    {
        StartCoroutine(LoadSceneCoroutine());
    }
    void Start(){
        
    }

    private IEnumerator LoadSceneCoroutine()
    {
        // Wait until CesiumGeoreference is initialized and active
        while (_cesiumGeoreference == null )
        {
            yield return null;
        }

        LoadScene();
    }
    public void LoadScene()
    {
      
    }

    public void exitTravelMode(){
        writeLatLonProgress();
        SceneManager.LoadScene("Start");
    }
    public void enterMapMode(){
        writeLatLonProgress();
        SceneManager.LoadScene("MapMode");
    }
    //note this save is not persistent. it writes to the current save state
    public void writeLatLonProgress(){
        double lat = _playerGlobalAnchor.longitudeLatitudeHeight.y;
        double lon = _playerGlobalAnchor.longitudeLatitudeHeight.x;

    }
    private void LLHToECEF(double latitude, double longitude, double height, out double x, out double y, out double z)
    {
        // WGS84 ellipsoid constants
        double a = 6378137.0; // Semi-major axis in meters
        double e2 = 6.69437999014e-3; // First eccentricity squared

        // Convert degrees to radians
        double latRad = latitude * Mathf.Deg2Rad;
        double lonRad = longitude * Mathf.Deg2Rad;

        double sinLat = Mathf.Sin((float)latRad);
        double cosLat = Mathf.Cos((float)latRad);
        double sinLon = Mathf.Sin((float)lonRad);
        double cosLon = Mathf.Cos((float)lonRad);

        // Radius of curvature in the prime vertical
        double N = a / Math.Sqrt(1 - e2 * sinLat * sinLat);

        // Calculate ECEF coordinates
        x = (N + height) * cosLat * cosLon;
        y = (N + height) * cosLat * sinLon;
        z = (N * (1 - e2) + height) * sinLat;
    }
}
