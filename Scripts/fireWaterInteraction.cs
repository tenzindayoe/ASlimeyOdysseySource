using UnityEngine;
using Obi;

public class FireExtinguisher : MonoBehaviour
{
    public ObiSolver solver; // Reference to the Obi Solver
    public GameObject fireObject; // The fire object to be extinguished

    private void OnEnable()
    {
        if (solver != null)
            solver.OnCollision += HandleCollision;
    }

    private void OnDisable()
    {
        if (solver != null)
            solver.OnCollision -= HandleCollision;
    }

    private void HandleCollision(ObiSolver solver, ObiNativeContactList contacts)
    {
        foreach (var contact in contacts)
        {
            if (contact.distance <= 0) // Ensure it's a valid collision
            {
                // Get the ObiColliderHandle for the contact
                var handle = ObiColliderWorld.GetInstance().colliderHandles[contact.bodyB];

                // Use the `owner` property to get the ObiColliderBase
                ObiColliderBase collider = handle?.owner;

                // Check if the collision is with this specific trigger collider
                if (collider != null && collider.gameObject == gameObject)
                {
                    Debug.Log("Water particle detected!");
                    
                    // Extinguish the fire
                    ExtinguishFire();
                }
            }
        }
    }

    private void ExtinguishFire()
    {
        if (fireObject != null)
        {
            fireObject.SetActive(false); // Turn off the fire
            Debug.Log("Fire extinguished!");
        }
    }
}
