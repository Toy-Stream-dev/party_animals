//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------

using UnityEngine;


public class ObjectOutOfBound : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private Transform objResetPoint;
    
    [SerializeField]
    private HandController rightHandController, leftHandController;
    
    //Hidden Variables
    //////////////////
    
    Rigidbody rb;


    //-----------------------------------------
    
    
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer("Player") && col.gameObject.layer != LayerMask.NameToLayer("Ragdoll") && col.tag == "CanBeGrabbed")
        {
            if(col.gameObject.GetComponent<Rigidbody>() != null)
            {
                if(rightHandController.gameObject.GetComponent<FixedJoint>() && rightHandController.gameObject.GetComponent<FixedJoint>().connectedBody == col.gameObject.GetComponent<Rigidbody>())
                {
                    rightHandController.gameObject.SetActive(false);
                    rightHandController.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                    rightHandController.GrabbedObject = null;
                    rightHandController.hasJoint = false;
                }
                
                if(leftHandController.gameObject.GetComponent<FixedJoint>() && leftHandController.gameObject.GetComponent<FixedJoint>().connectedBody == col.gameObject.GetComponent<Rigidbody>())
                {
                    leftHandController.gameObject.SetActive(false);
                    leftHandController.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                    leftHandController.GrabbedObject = null;
                    leftHandController.hasJoint = false;
                }
                
                rb = col.gameObject.GetComponent<Rigidbody>();
                rb.transform.position = objResetPoint.position;
            }
        }
    }
}
