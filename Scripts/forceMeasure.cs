using UnityEngine;

public class forceMeasure : MonoBehaviour
{
    public FixedJoint fixedJoint;


    // on force change print the force 
    void OnJointBreak(float breakForce)
    {
        Debug.Log("A joint has just been broken!, force: " + breakForce);
    }

    // Start is called before the first frame update
    void Start()
    {
        // get the fixed joint component
        fixedJoint = GetComponent<FixedJoint>();
        // set the break force to 1000
        fixedJoint.breakForce = 1000;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
