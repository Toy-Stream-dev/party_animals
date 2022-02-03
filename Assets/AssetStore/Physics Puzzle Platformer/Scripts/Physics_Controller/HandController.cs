//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------

using System.Collections;
using UnityEngine;

public class HandController : MonoBehaviour
{
    
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    public PlayerController Player_Controller;
    
    [SerializeField]
	public bool isLeft, hasJoint;
    
    [SerializeField]
    public Rigidbody GrabbedObject;
	
    //----------------------------------------

    void Update()
    {
        if(Player_Controller.useControls)
        {
            //Left Hand
            //On mouse release, destroy joint
            if(isLeft)
            {
                if(hasJoint && Input.GetAxisRaw(Player_Controller.reachLeft) == 0)
                {
                    this.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                    hasJoint = false;
                    GrabbedObject = null;
                }

                if(hasJoint && this.gameObject.GetComponent<FixedJoint>() == null)
                {
                    hasJoint = false;
                    GrabbedObject = null;
                }
            }

            //Right Hand
            //On mouse release, destroy joint
            if(!isLeft)
            {
                if(hasJoint && Input.GetAxisRaw(Player_Controller.reachRight) == 0)
                {
                    this.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                    hasJoint = false;
                    GrabbedObject = null;
                }

                if(hasJoint && this.gameObject.GetComponent<FixedJoint>() == null)
                {
                    hasJoint = false;
                    GrabbedObject = null;
                }
            }
        }
    }

    //Grab on collision
    void OnCollisionEnter(Collision col)
    {
        if(Player_Controller.useControls)
        {
            //Left Hand
            //On mouse pressed and collides, create joint
            if(isLeft)
            {
                if(col.gameObject.tag == "CanBeGrabbed" && col.gameObject.layer != LayerMask.NameToLayer(Player_Controller.ragdollLayer) && !hasJoint)
                {
                    if(Input.GetAxisRaw(Player_Controller.reachLeft) != 0 && !hasJoint)
                    {
                        hasJoint = true;
                        GrabbedObject = col.gameObject.GetComponent<Rigidbody>();
                        this.gameObject.AddComponent<FixedJoint>();
                        this.gameObject.GetComponent<FixedJoint>().breakForce = Mathf.Infinity;
                        this.gameObject.GetComponent<FixedJoint>().connectedBody = col.gameObject.GetComponent<Rigidbody>();
                    }
                }
                
            }

            //Right Hand
            //On mouse pressed and collides, create joint
            if(!isLeft)
            {
                if(col.gameObject.tag == "CanBeGrabbed" && col.gameObject.layer != LayerMask.NameToLayer(Player_Controller.ragdollLayer) && !hasJoint)
                {
                    if(Input.GetAxisRaw(Player_Controller.reachRight) != 0 && !hasJoint)
                    {
                        hasJoint = true;
                        GrabbedObject = col.gameObject.GetComponent<Rigidbody>();
                        this.gameObject.AddComponent<FixedJoint>();
                        this.gameObject.GetComponent<FixedJoint>().breakForce = Mathf.Infinity;
                        this.gameObject.GetComponent<FixedJoint>().connectedBody = col.gameObject.GetComponent<Rigidbody>();
                    }
                }
            }
        }
    }
}
