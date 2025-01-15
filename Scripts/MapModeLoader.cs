using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;

public class MapModeLoader : MonoBehaviour
{
    [SerializeField]
    private CesiumGlobeAnchor _playerGlobalAnchor;  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadMapRequirements(); 
    }

    public void loadMapRequirements(){
        
        
    }

    public void WritePositionToCurrentState(){
       
    }
    public void SaveActualPosition(){
      
    }
}
