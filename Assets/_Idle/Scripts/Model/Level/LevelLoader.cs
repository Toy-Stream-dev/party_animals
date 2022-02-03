using System;
using System.Collections.Generic;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using GeneralTools.Model;
using UnityEngine;
using UnityEngine.AI;

namespace _Idle.Scripts.Model.Level
{
	public class LevelLoader : BaseModel
	{
		public event Action<int> OnSpawnLevel;
		public event Action<int> OnRemoveLevel;
		
		public int CurrentLevelIndex { get; private set; }
		public LevelModel CurrentLevel { get; private set; }

		private LevelContainer _levelContainer;
		private List<LevelModel> _levels;
		private LevelParams[] _levelParams;

		public  override BaseModel Init()
		{
			_levelParams = GameBalance.Instance.LevelParams;
			
			_levelContainer = new LevelContainer();
			_levels = new List<LevelModel>(_levelParams.Length);

			return this;
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			
			_levelContainer.Update(deltaTime);
		}

		public void LoadLevel(int levelIndex)
		{
			CurrentLevelIndex = levelIndex - 1;
			LoadNextLevel();
		}

		public void LoadNextLevel()
		{
			if (_levelContainer.TryRemove(CurrentLevel))
			{
				NavMesh.RemoveAllNavMeshData();
				OnRemoveLevel?.Invoke(CurrentLevelIndex);
			}

			if (CurrentLevelIndex >= _levels.Capacity)
			{
				var rndIdx = _levels.Capacity > GameBalance.Instance.RandomLevelFromCount ? GameBalance.Instance.RandomLevelFromCount : _levels.Capacity;
				CurrentLevelIndex = _levels.Capacity - 1 - UnityEngine.Random.Range(0, ++rndIdx);
			}
			
			CurrentLevel = _levelContainer.Spawn(++CurrentLevelIndex);
			
			CurrentLevel.SetParams(_levelParams[CurrentLevelIndex - 1]);
			CurrentLevel.Start();
			CurrentLevel.SetState(LevelState.Progress);
			OnSpawnLevel?.Invoke(CurrentLevelIndex);
		}
	}
}