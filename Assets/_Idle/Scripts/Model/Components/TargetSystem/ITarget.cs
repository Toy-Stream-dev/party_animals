using System;
using UnityEngine;

namespace _Idle.Scripts.Model.Components
{
	public interface ITarget
	{
		public bool IsAvailable { get; }
		public Vector3 Position { get; }

		public event Action<ITarget> OnRemove;
	}
}