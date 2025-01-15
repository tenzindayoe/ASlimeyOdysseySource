using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
public class MapCameraScaleModifier : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Camera Orbit Scale Settings")]
    [SerializeField]
    private float minRadius = 1200f; 
    [SerializeField]
    private float maxRadius = 1700f;
    [SerializeField]
    private float defaultRadius = 1500f;
    [SerializeField]
    [Tooltip("Speed of the map zooming action")]
    private float zoomSpeed = 1f;

    [SerializeField]
    private CinemachineOrbitalFollow cinemachineOrbitalFollowScript; 
    [Header("Input Action")]
    [SerializeField]
    private InputActionReference radiusControlAction; // Reference to the 1D axis input action


    void Start()
    {
        if (cinemachineOrbitalFollowScript != null)
        {
            setRadius(defaultRadius);
        }
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (radiusControlAction != null && cinemachineOrbitalFollowScript != null)
        {
            // Get the input value (a float between -1 and 1 for a 1D axis)
            float inputValue = radiusControlAction.action.ReadValue<float>();

            // Calculate the new radius based on the input
            float newRadius = Mathf.Clamp(
                cinemachineOrbitalFollowScript.Radius + inputValue * Time.deltaTime * 100f * zoomSpeed, // Adjust speed multiplier as needed
                minRadius,
                maxRadius
            );

            // Set the new radius
            setRadius(newRadius);
        }
    }
    private void setRadius(float radius)
    {
        cinemachineOrbitalFollowScript.Radius = radius;
    }
}
