using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
// This class is used to store the entire state of the game at a particular point in time.
public class SaveState
{
    public DateTime SaveTime;

    public bool NewGame; // this is used to determine if the game is a new game or a loaded game.
    public string gameSeed;
    public bool InLake; 
    
    public string currentLocation;
    public int currentMaxWaterLevel; 
    public int currentMaxHealth; 
}

// public static class SaveStateUtils
// {
//     private static SaveStateScriptableObjScript saveStateScriptable;
    

//     // Ensure the ScriptableObject is loaded
//     private static void EnsureScriptableObjectLoaded()
//     {
//         if (saveStateScriptable == null)
//         {
//             saveStateScriptable = Resources.Load<SaveStateScriptableObjScript>("SaveStateScriptableObject");
//             if (saveStateScriptable == null)
//             {
//                 Debug.LogError("SaveStateScriptableObject not found in Resources!");
//             }
//         }
//     }

//     // Start a new game by resetting the ScriptableObject state
//     public static void StartNewGame()
//     {
//         EnsureScriptableObjectLoaded();
//         if (saveStateScriptable == null) return;

//         saveStateScriptable.SaveTime = DateTime.Now;
//         saveStateScriptable.gameSeed = HashGen.GenerateTimeAndRandBasedHash();
//         saveStateScriptable.NewGame = true;
//         saveStateScriptable.InLake = false;
//         saveStateScriptable.PlayerCurrentLakeState = null;
//         saveStateScriptable.Latitude = 45.46427f;
//         saveStateScriptable.Longitude = 9.18951f;

//         saveStateScriptable.Accomplishments.Clear();
//         saveStateScriptable.StoryLakeMissions = new List<StoryLakeMission>
//         {
//             new StoryLakeMission
//             {
//                 ID = 1,
//                 LakeName = "Lake Trinity",
//                 IsCompleted = false,
//                 Dependencies = new int[] { }
//             },
//             new StoryLakeMission
//             {
//                 ID = 2,
//                 LakeName = "Lake Casandra",
//                 IsCompleted = false,
//                 Dependencies = new int[] { 1 }
//             },
//             new StoryLakeMission
//             {
//                 ID = 3,
//                 LakeName = "Lake Victoria",
//                 IsCompleted = false,
//                 Dependencies = new int[] { 1, 2 }
//             }
//         };

//         Debug.Log("New game started in ScriptableObject!");
//     }

//     // Save the current state in the ScriptableObject to PlayerPrefs
//     public static void SaveGame()
//     {
//         EnsureScriptableObjectLoaded();
//         if (saveStateScriptable == null) return;

//         string saveStateJson = JsonUtility.ToJson(saveStateScriptable);
//         PlayerPrefs.SetString("SaveState", saveStateJson);
//         PlayerPrefs.Save();
//         Debug.Log("Game saved to PlayerPrefs!");
//     }

//     // Load the saved state from PlayerPrefs into the ScriptableObject
//     public static void LoadGame()
//     {
//         EnsureScriptableObjectLoaded();
//         if (saveStateScriptable == null) return;

//         string saveStateJson = PlayerPrefs.GetString("SaveState");
//         if (string.IsNullOrEmpty(saveStateJson))
//         {
//             Debug.LogError("No saved game found in PlayerPrefs!");
//             return;
//         }

//         JsonUtility.FromJsonOverwrite(saveStateJson, saveStateScriptable);
//         Debug.Log("Game loaded into ScriptableObject from PlayerPrefs!");
//     }

//     // Warning: Use only for debugging to clear the saved state in PlayerPrefs
//     public static void DeleteSave()
//     {
//         PlayerPrefs.DeleteKey("SaveState");
//         Debug.Log("Save state deleted from PlayerPrefs!");
//     }
// }

public static class SaveStateUtils
{
    private static SaveStateScriptableObjScript currentSaveState;
    private static SaveStateScriptableObjScript originalSaveState;

    // Ensure the ScriptableObjects are loaded
    private static void EnsureScriptableObjectsLoaded()
    {
        if (currentSaveState == null)
        {
            currentSaveState = Resources.Load<SaveStateScriptableObjScript>("CurrentSaveState");
            if (currentSaveState == null)
            {
                Debug.LogError("CurrentSaveGame ScriptableObject not found in Resources!");
            }
        }

        if (originalSaveState == null)
        {
            originalSaveState = Resources.Load<SaveStateScriptableObjScript>("OriginalSaveState");
            if (originalSaveState == null)
            {
                Debug.LogError("OriginalSaveGame ScriptableObject not found in Resources!");
            }
        }
    }

    public static int GetMaxHealth(){
        
        EnsureScriptableObjectsLoaded();
        if (currentSaveState == null) return 0;
        return currentSaveState.currentMaxHealth;
    }

    public static int GetMaxWaterLevel(){
        EnsureScriptableObjectsLoaded();
        if (currentSaveState == null) return 0;
        return currentSaveState.currentMaxWaterLevel;
    }
    // Start a new game by copying data from OriginalSaveGame to CurrentSaveGame
    public static void StartNewGame()
    {
        EnsureScriptableObjectsLoaded();
        if (currentSaveState == null || originalSaveState == null) return;

        // Copy data from OriginalSaveGame to CurrentSaveGame
        currentSaveState.SaveTime = DateTime.Now;
        currentSaveState.gameSeed = originalSaveState.gameSeed;
        currentSaveState.NewGame = true;
        currentSaveState.InLake = originalSaveState.InLake;
        currentSaveState.currentMaxWaterLevel = originalSaveState.currentMaxWaterLevel;
        currentSaveState.currentMaxHealth = originalSaveState.currentMaxHealth;
        currentSaveState.currentLocation = originalSaveState.currentLocation;



        Debug.Log("New game initialized. CurrentSaveGame updated from OriginalSaveGame.");
    }

    // Save the current state in CurrentSaveGame to PlayerPrefs
    public static bool SaveGame()
    {
        
        if (currentSaveState == null) return false;
        string saveStateJson = JsonUtility.ToJson(currentSaveState);
        PlayerPrefs.SetString("SaveState", saveStateJson);
        PlayerPrefs.Save();
        Debug.Log("Game saved to PlayerPrefs!");
        return true;
    }

    // Load the saved state from PlayerPrefs into CurrentSaveGame
    public static bool LoadGame()
    {
        EnsureScriptableObjectsLoaded();
        if (currentSaveState == null) return false;

        string saveStateJson = PlayerPrefs.GetString("SaveState");
        if (string.IsNullOrEmpty(saveStateJson))
        {
            //Debug.LogError("No saved game found in PlayerPrefs!");
            return false;
        }

        JsonUtility.FromJsonOverwrite(saveStateJson, currentSaveState);
        Debug.Log("Game loaded into CurrentSaveGame from PlayerPrefs!");
        return true;
    }

    // Warning: Use only for debugging to clear the saved state in PlayerPrefs
    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey("SaveState");
        Debug.Log("Save state deleted from PlayerPrefs!");
    }
    public static bool CheckIfSaveExists()
    {
        if (PlayerPrefs.HasKey("SaveState"))
        {
            Debug.Log("Save exists");
            return true;
        }
        else
        {
            Debug.Log("Save does not exist");
            return false;
        }
    }


    public static  SaveStateScriptableObjScript GetCurrentSaveState()
    {
        return currentSaveState;
    }
}