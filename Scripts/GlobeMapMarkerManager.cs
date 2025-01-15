
using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;

[Serializable]
public enum GameLocation{
    HomeTown, 
    Lake1,
    Lake2



}
public class GlobeMapMarkerManager : MonoBehaviour
{

    public SceneSwitcher sceneSwitcher;

    public  GameLocation currentLocation = GameLocation.HomeTown;
    public GameLocation nextLocation = GameLocation.Lake2;
    public DOTweenAnimation PlayerTween; 

    private Dictionary<GameLocation, GameLocation> NextLocation = new Dictionary<GameLocation, GameLocation>
    {
        {GameLocation.HomeTown, GameLocation.Lake1},
        {GameLocation.Lake1, GameLocation.Lake2},
        {GameLocation.Lake2, GameLocation.HomeTown}
    }; 

    private Dictionary<GameLocation, GameObject> LocationObjects = new Dictionary<GameLocation, GameObject>
    {
        {GameLocation.HomeTown, null},
        {GameLocation.Lake1, null},
        {GameLocation.Lake2, null}
    };

    private string getNameFromGameLocationEnum(GameLocation en){
        return en.ToString();
    }

private GameLocation getGameLocEnumFromString(string strName){
    if(Enum.TryParse(strName, out GameLocation location)){
        return location;
    }
    else{
        Debug.LogError($"Invalid GameLocation name: {strName}. Defaulting to HomeTown.");
        return GameLocation.HomeTown;
    }
}
    

    public GameObject HomeTown; 
    public GameObject Lake1;
    public GameObject Lake2;


    void Start(){

        LocationObjects[GameLocation.HomeTown] = HomeTown;
        LocationObjects[GameLocation.Lake1] = Lake1;
        LocationObjects[GameLocation.Lake2] = Lake2;
        loadInitial(); 
        //travelToNext(); 
    }

       
       

    public void loadInitial(){
        // for testing
        SaveStateUtils.StartNewGame(); 
        //SaveStateUtils.GetCurrentSaveState().currentLocation = "Lake1";
        
        //
        
        string locName = SaveStateUtils.GetCurrentSaveState().currentLocation; 
        GameLocation locationEnum = getGameLocEnumFromString(locName);
        currentLocation = locationEnum;

        GameObject currentObj = LocationObjects[currentLocation];

        PlayerTween.transform.gameObject.transform.position = currentObj.transform.position; 
        PlayerTween.transform.gameObject.transform.rotation = currentObj.transform.rotation; 
        
        // PlayerTween.transform.DOMove(currentObj.transform.position, 0f).SetEase(Ease.InOutSine);
        // PlayerTween.transform.DORotate(currentObj.transform.rotation.eulerAngles, 0f).SetEase(Ease.InOutSine);
       

    }
    public void travelToNext()
    {
        // Determine the next location
        nextLocation = NextLocation[currentLocation];

        // Get the position and rotation of the next location
        Vector3 targetPosition = LocationObjects[nextLocation].transform.position;
        Vector3 targetRotation = LocationObjects[nextLocation].transform.rotation.eulerAngles;

        // Animate the player's movement and rotation using DOTween
        Sequence travelSequence = DOTween.Sequence();

        // Add the movement animation
        travelSequence.Append(
            PlayerTween.transform.DOMove(targetPosition, 5f).SetEase(Ease.InOutSine)
        );

        // Add the rotation animation
        travelSequence.Join(
            PlayerTween.transform.DORotate(targetRotation, 5f).SetEase(Ease.InOutSine)
        );

        // When the animation is complete, call travelDone
        travelSequence.OnComplete(() =>
        {
            // Update the current location to the new location
            currentLocation = nextLocation;

            // Save the game state
            SaveStateUtils.SaveGame();

            // Call the travelDone method
            travelDone();
        });

        // Start the sequence
        travelSequence.Play();
    }


    public void travelDone(){
        Debug.Log("Travel Done");
        string sceneName = getNameFromGameLocationEnum(currentLocation);
        sceneSwitcher.SwitchScene(sceneName, "Best of Luck in your journey Wealy!");
    

    }






    
}
