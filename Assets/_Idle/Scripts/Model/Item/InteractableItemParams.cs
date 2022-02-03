using System;
using _Idle.Scripts.Enums;
using Sirenix.OdinInspector;

namespace _Idle.Scripts.Model.Item
{
	[Serializable]
	public class InteractableItemParams
	{
		public InteractableItemType ItemType; 
		public float InteractableTime = 1.0f;
		public bool IsWeapon = true;

		[ShowIf(nameof(IsWeapon))]
		public WeaponType WeaponType;
		[ShowIf(nameof(WeaponType), WeaponType.Range)]
		public int AmmoAmount;
		[ShowIf(nameof(WeaponType), WeaponType.Range)]
		public float ShootInterval = 1.0f;
		[ShowIf(nameof(IsWeapon))]
		public float PunchPower;
	}
}