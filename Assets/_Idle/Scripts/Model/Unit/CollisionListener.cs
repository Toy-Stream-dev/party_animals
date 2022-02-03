using System;
using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.Model.Unit
{
	[RequireComponent(typeof(Collider))]
	public class CollisionListener : BaseBehaviour
	{
		public LayerMask OnTriggerStayLayer;
		public LayerMask OnCollisionStayLayer;
		
		public event Action<Collider> OnOnTriggerEnter;
		public event Action<Collider> OnOnTriggerExit;
		public event Action<Collider> OnOnTriggerStay = delegate { };
		public event Action<Collision> OnOnCollisionEnter;
		public event Action<Collision> OnOnCollisionExit;
		public event Action<Collision> OnOnCollisionStay = delegate { };
		
		public UnitModel UnitModel { get; set; } 

		protected void OnTriggerEnter(Collider other)
		{
			OnOnTriggerEnter?.Invoke(other);
		}

		protected void OnTriggerExit(Collider other)
		{
			OnOnTriggerExit?.Invoke(other);
		}

		private void OnTriggerStay(Collider other)
		{
			if (((1 << other.gameObject.layer) & OnTriggerStayLayer) == 0)
				return;
			
			OnOnTriggerStay.Invoke(other);
		}

		protected void OnCollisionEnter(Collision other)
		{
			OnOnCollisionEnter?.Invoke(other);
		}

		protected void OnCollisionExit(Collision other)
		{
			OnOnCollisionExit?.Invoke(other);
		}

		// private void OnCollisionStay(Collision other)
		// {
		// 	if (((1 << other.gameObject.layer) & OnCollisionStayLayer) == 0)
		// 		return;
		// 	
		// 	OnOnCollisionStay?.Invoke(other);
		// }
	}
}