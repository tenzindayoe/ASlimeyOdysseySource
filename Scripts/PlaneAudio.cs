using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaneAudio : MonoBehaviour
{
    public FlyingMachineController flyingMachineController; // Reference to the FlyingMachineController
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to the GameObject
        audioSource = GetComponent<AudioSource>();

        if (flyingMachineController == null)
        {
            Debug.LogError("FlyingMachineController is not assigned!");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing!");
        }
    }

    void Update()
    {
        if (flyingMachineController != null && audioSource != null)
        {
            // Get the current and max speed from the FlyingMachineController
            float currentSpeed = flyingMachineController.CurrentSpeed;
            float maxSpeed = flyingMachineController.maxSpeed;

            // Calculate the volume based on the speed factor (currentSpeed / maxSpeed)
            float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);

            // Set the AudioSource volume (adjust this multiplier if needed)
            audioSource.volume = speedFactor;
        }
    }
}
