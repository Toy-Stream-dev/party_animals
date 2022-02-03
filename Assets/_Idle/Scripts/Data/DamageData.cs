using System;
using _Idle.Scripts.Interfaces;
using UnityEngine;

namespace _Idle.Scripts.Data
{
	[Serializable]
	public class DamageData : IDamageData
	{
		[SerializeField] private int _amount;
		[SerializeField] private DamageType _damageType;

		public int Amount
		{
			get => _amount;
			private set => _amount = value;
		}

		public DamageType Type
		{
			get => _damageType;
			private set => _damageType = value;
		}

		public DamageData(int amount)
		{
			_amount = amount;
		}

		public DamageData(int amount, DamageType damageType) : this(amount)
		{
			_damageType = damageType;
		}

		public override string ToString()
		{
			return $"DamageData: Amount - {Amount}, Type - {Type}";
		}
	}
}