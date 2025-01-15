using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartNewGame(){
        SaveStateUtils.StartNewGame();
        // should go to the village scene
        SceneManager.LoadScene("Hometown");
    }
    
    public bool checkIfSaveAvailable(){
        return SaveStateUtils.CheckIfSaveExists();
    }

    public bool LoadGame(){
        bool loadAvailable = SaveStateUtils.LoadGame();
        if (!loadAvailable){
            return false;
        }

        // should go to the village scene
        if (SaveStateUtils.GetCurrentSaveState().InLake == true)
        {
            // should go to the lake scene

        }else{
            SceneManager.LoadScene("TravelScene");
        }
        return true;
    }
}
