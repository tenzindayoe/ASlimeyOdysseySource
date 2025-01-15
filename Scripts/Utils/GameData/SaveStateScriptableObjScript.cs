using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SaveStateScriptableObjScript", menuName = "Scriptable Objects/SaveStateScriptableObjScript")]
public class SaveStateScriptableObjScript : ScriptableObject
{
    public DateTime SaveTime;
    public bool NewGame;
    public string gameSeed;
    public bool InLake;
    public String currentLocation;
    public int currentMaxWaterLevel; 
    public int currentMaxHealth; 
}


// public enum GameLocation{
//     HomeTown, 
//     Sky1,
//     Lake1,
//     Sky2, 
//     Lake2



// }