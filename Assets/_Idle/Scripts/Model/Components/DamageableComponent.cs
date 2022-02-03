using _Idle.Scripts.Interfaces;
using GeneralTools.Model;
using UnityEngine;

namespace _Idle.Scripts.Model.Components
{
	public class DamageableComponent : BaseComponent, IDamageable
	{
		private readonly HealthComponent _health;
		
		public DamageableComponent(BaseModel model) : base(model)
		{
			_health = model.Get<HealthComponent>();
			
			Debug.Assert(_health != null, $"{nameof(HealthComponent)} don't exist!");
		}

		public void TakeDamage(IDamageData damageData)
		{
			if (_health.CurrentHealth <= 0)
				return;
			
			_health.ReduceHealth(damageData.Amount);
		}
	}
}