using System;
using _Idle.Scripts.Interfaces;
using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.Model.Components
{
	public class BaseDamageableTarget : BaseBehaviour, IDamageableTarget
	{
		[SerializeField] private bool _availableTarget = true;

		public TargetGroup TargetGroup { get; protected set; }
		public bool IsAvailable => _availableTarget;
		public Vector3 Position { get; protected set; }
		public event Action<ITarget> OnRemove;

		public IDamageable Damageable { get; protected set; }

		public void SetTarget(IDamageableTarget target)
		{
		}
	}
}