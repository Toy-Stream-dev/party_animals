using UnityEngine;

namespace _Idle.Scripts.Model.Unit
{
	public class CollisionArgs
	{
		public Component Sender;
		public Collider Collider;
		public Vector3 ContactPoint;

		public CollisionArgs(Component sender, Collider collider)
		{
			Sender = sender;
			Collider = collider;
		}
	}
}