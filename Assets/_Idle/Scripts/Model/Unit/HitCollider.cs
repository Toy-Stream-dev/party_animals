using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.Model.Unit
{
	[RequireComponent(typeof(Collider))]
	public abstract class HitCollider : BaseBehaviour
	{
		protected IHitListener _listener;

		public virtual void AddListener(IHitListener listener)
		{
			_listener = listener;
		}
		
		protected abstract void OnTriggerEnter(Collider other);
	}
}