using UnityEngine;

public class LakeMarker : MonoBehaviour
{
    private bool isFocused = false; // Tracks whether this object is focused
    private bool isSelected = false; 
    public bool IsSelected { get { return isSelected; } }
    private string lakeName; 

    public void setLakeName(string name){
        lakeName = name;
    }
    public string getLakeName(){
        return lakeName;
    }
    public void unselect(){
        isSelected = false; 
    }
    public void select(){
        isSelected = true;
    }
    public void OnLeftClick()
    {
        if (isFocused)
        {
            select(); 
        }
    }

    public void OnFocusEnter()
    {
        if (!isFocused)
        {
            isFocused = true;
            Debug.Log("Object is now focused");
            //double the size of the object 
            this.gameObject.transform.localScale *= 2; 
            // Add any additional logic when the object is focused
        }
    }

    public void OnFocusExit()
    {
        if (!isSelected && isFocused)
        {
            isFocused = false;
            Debug.Log("Object lost focus");
            this.gameObject.transform.localScale /= 2;
            // Add any additional logic when the object loses focus
        }
    }


    
}
