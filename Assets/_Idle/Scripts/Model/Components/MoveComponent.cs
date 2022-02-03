using System;
using _Idle.Scripts.Model.Unit;
using GeneralTools.Model;
using UnityEngine;

namespace _Idle.Scripts.Model.Components
{
	public class MoveComponent : BaseComponent<UnitModel>
	{
		public event Action OnEnd;
		
		public float MoveSpeed { get; set; } = 1.0f;

		private Vector3 _deltaPosition;
        
		public MoveComponent(UnitModel model) : base(model)
		{
		}

		public void Move(Vector3 direction, float deltaTime)
		{
			_deltaPosition = direction.normalized * (MoveSpeed * deltaTime);
			// Debug.Log($"{direction} | {direction.normalized} | {MoveSpeed} | {_deltaPosition} | {deltaTime}");
			Model.Move(_deltaPosition);
		}

		public override void End()
		{
			OnEnd?.Invoke();
			base.End();
		}
	}
}