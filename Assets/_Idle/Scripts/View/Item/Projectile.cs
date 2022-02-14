using System;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.View.Unit;
using DG.Tweening;
using GeneralTools;
using GeneralTools.Pooling;
using UnityEngine;

namespace _Idle.Scripts.View.Item
{
	public class Projectile : BaseBehaviour
	{
		public InteractableItemType ProjectileType;
		
		[SerializeField] private Rigidbody _rigidbody;
		[SerializeField] private Collider _collider;

		private float _punchPower;

		public void Throw(Vector3 direction, float punchPower)
		{
			_punchPower = punchPower;
			_rigidbody.AddForce(direction.normalized * GameBalance.Instance.ThrowForce);

			DOTween.Sequence().AppendInterval(0.12f)
				.OnComplete(EnableCollider);
		}

		public void DisableCollider()
		{
			_collider.enabled = false;
		}

		private void EnableCollider()
		{
			_collider.enabled = true;
		}

		private void OnCollisionEnter(Collision other)
		{
			DisableCollider();
			
			var unit = other.gameObject.GetComponentInParent<UnitView>();
			if (unit == null)
			{
				_rigidbody.velocity = Vector3.zero;
				this.PushToPool();
				return;
			}

			var args = new ProjectileArgs
			{
				Velocity = _rigidbody.velocity,
				Power =  _punchPower,
				Collision = other,
				ProjectileType = ProjectileType
			};
			
			_rigidbody.velocity = Vector3.zero;
			unit.Model.ProjectilePush(args);
			this.PushToPool();
		}
	}

	[Serializable]
	public class ProjectileArgs
	{
		public Vector3 Velocity;
		public float Power;
		public Collision Collision;
		public InteractableItemType ProjectileType;
	}
}