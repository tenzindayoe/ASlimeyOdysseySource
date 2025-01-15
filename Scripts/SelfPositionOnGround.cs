using UnityEngine;
using CesiumForUnity;
using Unity.Mathematics;

public class SelfPositionOnGround : MonoBehaviour
{
    [Header("Cesium References")]
    public GameObject groundObj; // Reference to CesiumGeoreference
    public CesiumGlobeAnchor playerGlobeAnchor;  // Reference to the player's GlobeAnchor
    public CesiumGeoreference georeference; 

    /// <summary>
    /// Updates the position of the GameObject to the ground level at the player's latitude and longitude.
    /// </summary>
    ///     
    

    public void UpdatePositionToPlayerGround()
    {
        // Get latitude and longitude from the player's GlobeAnchor
        double latitude = playerGlobeAnchor.longitudeLatitudeHeight.y;
        double longitude = playerGlobeAnchor.longitudeLatitudeHeight.x;


        // Convert groundCoordinates to earth-centered, earth-fixed (ECEF) position
        double3 ecefPosition = CesiumForUnity.CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(longitude, latitude,100));
        

        // Convert ECEF position to Unity world coordinates
        double3 unityPositionD3 = georeference.TransformEarthCenteredEarthFixedPositionToUnity(ecefPosition);

        Vector3 unityPositionV3 = new Vector3((float)unityPositionD3.x, (float)unityPositionD3.y, (float)unityPositionD3.z);
        // Set the transform of groundObj to that position
        groundObj.transform.position = unityPositionV3;



    }
    void Update(){
        UpdatePositionToPlayerGround();
    }
}
