using _Idle.Scripts.Interfaces;
using GeneralTools.Model;

namespace _Idle.Scripts.Model.Components
{
	public class DamagedComponent : BaseComponent, IDamaged
	{
		public IDamageData DamageData { get; private set; }
		
		public DamagedComponent(BaseModel model) : base(model)
		{
		}

		public void SetDamageData(IDamageData data)
		{
			DamageData = data;
		}

		public void BringDamage(IDamageable target)
		{
			target?.TakeDamage(DamageData);
		}
	}
}