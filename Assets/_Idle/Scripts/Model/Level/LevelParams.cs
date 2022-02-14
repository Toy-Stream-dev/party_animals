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
		[SerializeField]
		[HideIf(nameof(_isTutorial))]
		[MinMaxSlider(minValue: 2, maxValue: 6)]
		private Vector2Int _playersCount = new Vector2Int(2, 4);
		
		public int LevelIndex => _levelIndex;

		public bool IsTutorial => _isTutorial;

		public Vector2Int PlayersCount => _playersCount;
	}
}