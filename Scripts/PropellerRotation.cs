using UnityEngine;

public class PropellerRotation : MonoBehaviour
{
    public float rotationsPerSecond; // Target rotations per second
    public bool spin = false; // Determines whether the propeller is spinning

    public float rotationSpeed; // Current rotation speed in degrees per second
    public float lerpSpeed = 2f; // Speed of interpolation (higher is faster)
    private float targetSpeed; // Target rotation speed

    void Start()
    {
        targetSpeed = 0f; // Start with no rotation
        rotationSpeed = 0f; // Current speed is also 0
    }

    private void Update()
    {
        // Gradually interpolate towards the target speed
        rotationSpeed = Mathf.Lerp(rotationSpeed, targetSpeed, lerpSpeed * Time.deltaTime);

        if (rotationSpeed > 0.01f) // Avoid very small values for efficiency
        {
            // Rotate the propeller around its local X-axis
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void startSpinning()
    {
        spin = true;
        targetSpeed = rotationsPerSecond * 360; // Set the target speed in degrees per second
    }

    public void stopSpinning()
    {
        spin = false;
        targetSpeed = 0f; // Gradually slow down to stop
    }
}
