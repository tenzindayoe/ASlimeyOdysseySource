using UnityEngine;

public class PositionFollower : MonoBehaviour
{
    [Header("Target Object")]
    public GameObject targetObject; // The object whose position we want to follow

    /// <summary>
    /// Updates this GameObject's position to match the target object's position.
    /// </summary>
    public void UpdatePosition()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object is not assigned.");
            return;
        }

        // Set this GameObject's position to the target's world position
        transform.position = targetObject.transform.position;

        // Log for debugging
        Debug.Log($"Position updated to target object at: {transform.position}");
    }
}
