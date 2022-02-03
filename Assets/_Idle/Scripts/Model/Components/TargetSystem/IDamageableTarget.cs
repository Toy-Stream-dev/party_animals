using System;
using _Idle.Scripts.Interfaces;

namespace _Idle.Scripts.Model.Components
{
	public interface IDamageableTarget : ITarget
	{
		public IDamageable Damageable { get; }

		public void SetTarget(IDamageableTarget target);
	}
}