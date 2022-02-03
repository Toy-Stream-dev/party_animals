//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------

using _Idle.Scripts.Enums;
using _Idle.Scripts.Tools;
using Unity.Mathematics;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField] private AnimationControllerType _animationControllerType;
    
    [SerializeField]
    private bool invertRotation;
    
    [SerializeField]
    private ConfigurableJoint thisJoint;
    
    [SerializeField]
    private Transform animationTarget;


    public AnimationControllerType AnimationControllerType
    {
	    get => _animationControllerType;
	    set => _animationControllerType = value;
    }
    
    //Hidden Variables
    //////////////////
    
    private Quaternion Rotation;
    
    
    void Start()
    {
        // Rotation = Quaternion.Inverse(animationTarget.localRotation);
        Rotation = thisJoint.transform.localRotation;
    }

    private void LateUpdate()
    {
	    switch (_animationControllerType)
	    {
		    case AnimationControllerType.Physics:
			    if(invertRotation)
			    {
				     thisJoint.SetTargetRotationLocal(Quaternion.Inverse(animationTarget.localRotation), Rotation);
				    // thisJoint.SetTargetRottion(Rotation, Quaternion.Inverse(animationTarget.localRotation));
				    // thisJoint.targetRotation = Quaternion.Inverse(animationTarget.localRotation * Rotation);
			    }
        
			    else
			    {
				     thisJoint.SetTargetRotationLocal(animationTarget.localRotation, Rotation);
				    // thisJoint.SetTargetRotation(Rotation, animationTarget.localRotation);
				    // thisJoint.targetRotation = animationTarget.localRotation * Rotation;
			    }
			    break;
		    case AnimationControllerType.Transform:
			    if(invertRotation)
			    {
				    // thisJoint.SetTargetRotationLocal(Quaternion.Inverse(animationTarget.localRotation), Rotation);
				    // thisJoint.SetTargetRottion(Rotation, Quaternion.Inverse(animationTarget.localRotation));
				    thisJoint.transform.localRotation = Quaternion.Inverse(animationTarget.localRotation * Rotation);
			    }
        
			    else
			    {
				    // thisJoint.SetTargetRotationLocal(animationTarget.localRotation, Rotation);
				    // thisJoint.SetTargetRotation(Rotation, animationTarget.localRotation);
				    thisJoint.transform.localRotation = animationTarget.localRotation * Rotation;
			    }
			    break;
	    }
    }
}
