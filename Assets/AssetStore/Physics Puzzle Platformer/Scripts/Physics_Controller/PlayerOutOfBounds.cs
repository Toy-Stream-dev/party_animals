//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------

using System.Collections;
using UnityEngine;

public class PlayerOutOfBounds : MonoBehaviour
{
    
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private PlayerController playerController;
    
    [SerializeField]
    private GameObject ragdollPlayer;
    
    [SerializeField]
    private GameObject ragdollRoot;
    
    [SerializeField]
    private HandController rightHandController, leftHandController;
    
    [SerializeField]
    private Transform resetPoint;
    
    [SerializeField]
    private bool instantCameraUpdate = false;
    
    
    //Hidden Variables
    //////////////////
    
    Camera cam;
    bool checkedTrigger;
    Rigidbody[] ragdollParts;
    Vector3 storedVelocity;
    
    
    //---------------------------------------
    
    
    void Awake()
    {
        cam = Camera.main;
    }
    
    
    void OnTriggerEnter(Collider col)
    {
        if(!checkedTrigger)
        {
            if(col.gameObject.layer == LayerMask.NameToLayer("Player") || col.gameObject.layer == LayerMask.NameToLayer("Ragdoll"))
            {
                checkedTrigger = true;
                
                playerController.useControls = false;
                
                if(rightHandController.gameObject.GetComponent<FixedJoint>())
                {
                    rightHandController.gameObject.SetActive(false);
                    rightHandController.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                    rightHandController.GrabbedObject = null;
                    rightHandController.hasJoint = false;
                }
                
                if(leftHandController.gameObject.GetComponent<FixedJoint>())
                {
                    leftHandController.gameObject.SetActive(false);
                    leftHandController.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                    leftHandController.GrabbedObject = null;
                    leftHandController.hasJoint = false;
                }
                
                if(ragdollPlayer != null)
                {
                    ragdollParts = ragdollPlayer.GetComponentsInChildren<Rigidbody>();
                    
                    //Deactivate physics and store velocity
                    foreach (Rigidbody physics in ragdollParts)
                    {
                        storedVelocity = physics.velocity;
                        physics.isKinematic = true;
                    }
                    
                    //Record camera current offset
                    var cameraOffset = new Vector3(cam.transform.position.x - ragdollRoot.transform.position.x, cam.transform.position.y - ragdollRoot.transform.position.y, cam.transform.position.z - ragdollRoot.transform.position.z);
                    
                    
                    //Set player to new position
                    ragdollRoot.transform.localPosition = Vector3.zero;
                    ragdollPlayer.transform.position = resetPoint.position;
                    
                    //Re-activate physics and apply stored velocity
                    foreach (Rigidbody physics in ragdollParts)
                    {
                        physics.isKinematic = false;
                        physics.velocity = storedVelocity;
                    }
                    
                    
                    //Apply camera offset to new position
                    if(instantCameraUpdate)
                    {
                        cam.transform.position = ragdollRoot.transform.position + cameraOffset;
                    }
                }
                
                checkedTrigger = false;
                playerController.useControls = true;
                
                rightHandController.gameObject.SetActive(true);
                leftHandController.gameObject.SetActive(true);
            }
        }
    }
}
