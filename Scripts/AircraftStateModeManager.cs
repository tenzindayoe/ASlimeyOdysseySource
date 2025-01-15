using UnityEngine;

public class AircraftStateModeManager : MonoBehaviour
{
    public string currentMode = "AircraftMode";
    public string[] getModes(){
        return new string[] {"MapMode", "AircraftMode"};
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void switchToMapMode(){

    }
    public void switchToAircraftMode(){

    }

}
