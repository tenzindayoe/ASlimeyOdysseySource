using UnityEngine;
using UnityEngine.Events;

public class UniSignals : MonoBehaviour{

    public UnityEvent<int> atSignal = new UnityEvent<int>();
    public static UniSignals Instance;
    void Awake(){
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(this.gameObject); // Destroy duplicate instances
        }
    }

   

    public void EmitSignal(int signal){
        atSignal.Invoke(signal);
    }

    public void ListenToSignal(UnityAction<int> action){
        atSignal.AddListener(action);
    }

    public void StopListeningToSignal(UnityAction<int> action){
        atSignal.RemoveListener(action);
    }

    
    
}