using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DoTweenSeperateObjectOrchestrator : MonoBehaviour
{
    public List<GameObject> managedObjects = new List<GameObject>();

    [TextArea(3, 10)]
    public string description = "Plays the DOTween animations on this GameObject. Use the 'join' flag to determine whether animations are joined or appended. Pause and resume depend on the current Sequence state.";
    [TextArea(3, 10)]
    public string note = "Ideal when all animations are played together or one after another. Note that the sequential is quite complicated since the order is based on the list of the objects and then the order of the DOTweenAnimations on the object.";


    public void flipStates(){
        foreach(GameObject go in managedObjects){
            foreach(DOTweenAnimation doTween in go.GetComponents<DOTweenAnimation>()){
                if(doTween != null){
                    doTween.tween.TogglePause(); 
                }
            }
        }
    }


    
    //not needed in the case of autoplay
    
}
