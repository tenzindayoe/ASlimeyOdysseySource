using UnityEngine; 

public class SlimedBirds:MonoBehaviour{


    public float freeAtScale = 0.1f; 
    void Start(){
        
    }

    void Update(){
        if(this.transform.localScale.x < freeAtScale){
            freeBird(); 
        }
    }

    public void freeBird(){

        
    }




}