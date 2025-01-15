using UnityEngine;

[CreateAssetMenu(fileName = "GamePresets", menuName = "Scriptable Objects/GamePresets")]
public class GamePresets : ScriptableObject
{
    public float startingLatitude;
    public float startingLongitude;

    public string gameDefaultSeed; 

    public string baseURL;

}
