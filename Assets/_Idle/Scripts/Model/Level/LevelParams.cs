using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Idle.Scripts.Model.Level
{
	[Serializable]
	public class LevelParams
	{
		[SerializeField] 
		private int _levelIndex;
		[SerializeField] 
		private bool _isTutorial;
		
		public int LevelIndex => _levelIndex;

		public bool IsTutorial => _isTutorial;
	}
}