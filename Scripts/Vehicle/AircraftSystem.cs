using Unity.VisualScripting;
using UnityEngine;

public class AircraftSystem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerStateMachine = other.GetComponent<PlayerStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.IsAircraftNear = true;
                DisplayMessage();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerStateMachine = other.GetComponent<PlayerStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.IsAircraftNear = false; 
                ClearMessage();
            }
        }
    }
    public void DisplayMessage()
    {
        Debug.Log("Press E to enter the aircraft."); // Replace with actual UI message
    }

    public void ClearMessage()
    {
        Debug.Log("Message cleared."); // Replace with UI message clearing logic
    }

    public void EnterAircraft()
    {
        Debug.Log("Entering the aircraft!"); // Placeholder for entering logic
        // Add functionality to handle player entering the aircraft
    }
}
