using System;
using GeneralTools.Model;
using UnityEngine;

namespace _Idle.Scripts.Model.Components
{
	public class HealthComponent : BaseComponent
	{
		private int _fullHealth;
		private int _currentHealth;

		public event Action<int, int> HealthChanged;
		public event Action HealthIsOver;

		public float Normalize { get; private set; }
		
		public bool IsFull => _currentHealth == _fullHealth;

		public int CurrentHealth
		{
			get => _currentHealth;
			private set => _currentHealth = value;
		}

		public int FullHealth
		{
			get => _fullHealth;
			private set => _fullHealth = value;
		}

		public HealthComponent(BaseModel model) : base(model)
		{
			SetFullHealth();
		}

		public void SetFullHealth()
		{
			_currentHealth = _fullHealth;
		}

		public void SetAllHealthValues(int currentValue, int fullValue)
		{
			_currentHealth = currentValue;
			_fullHealth = fullValue;
			ChangeHealth(0);
		}

		public void IncreaseHealth(int amount)
		{
			ChangeHealth(amount);
		}

		public void ReduceHealth(int amount)
		{
			ChangeHealth(-amount);
		}

		private void ChangeHealth(int changeAmount)
		{
			_currentHealth += changeAmount;
			_currentHealth = Mathf.Clamp(_currentHealth, 0, _fullHealth);
			
			Normalize = 1.0f / _fullHealth * _currentHealth;

			HealthChanged?.Invoke(_currentHealth, changeAmount);

			if (_currentHealth > 0) 
				return;
			
			HealthIsOver?.Invoke();
			HealthIsOver = null;
		}
	}
}