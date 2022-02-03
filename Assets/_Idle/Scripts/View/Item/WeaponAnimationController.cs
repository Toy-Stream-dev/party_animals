using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.View.Item
{
	public class WeaponAnimationController : BaseBehaviour
	{
		[SerializeField]
		private Transform animationTarget;
		
		[SerializeField]
		private bool invertRotation;
		
		private Quaternion Rotation;

		public Transform RightHand;
		public Transform LeftHand;
		
		private void Start()
		{
			Rotation = Quaternion.Inverse(animationTarget.localRotation);
		}
		
		private void FixedUpdate()
		{
			transform.localPosition = animationTarget.localPosition;
			
			if (invertRotation)
			{
				transform.rotation = Quaternion.Inverse(animationTarget.localRotation /** RightHand.rotation*/);
			}
			else
			{
				transform.rotation = animationTarget.localRotation /** RightHand.rotation*/;
			}
		}

		public void SetTarget(Transform target)
		{
			animationTarget = target;
		}
	}
}