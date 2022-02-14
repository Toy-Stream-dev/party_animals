using System;
using System.Collections.Generic;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.View.Level;
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
		private int _lastLevelId;

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
			RemoveLevel();
			CurrentLevelIndex = levelIndex;
			LoadNextLevel();
		}

		public void LoadNextLevel()
		{
			if (DevFlags.DEV_TOOLS)
			{
				var view = MainGame.Root.GetComponentInChildren<LevelView>();
				view.Init();
				CurrentLevel = _levelContainer.Create();
				CurrentLevel.SetView(view);
				CurrentLevelIndex = view.LevelIndex - 1;
			}
			else
			{
				if (CurrentLevelIndex != 0)
				{
					CurrentLevelIndex = UnityEngine.Random.Range(1, _levels.Capacity);

					while (_lastLevelId == CurrentLevelIndex)
					{
						CurrentLevelIndex = UnityEngine.Random.Range(1, _levels.Capacity);
					}
				}
			
				_lastLevelId = CurrentLevelIndex;
				CurrentLevel = _levelContainer.Spawn(CurrentLevelIndex);
				if (CurrentLevel == null || CurrentLevel.View == null)
					LoadNextLevel();
			}
			
			CurrentLevel.SetParams(_levelParams[CurrentLevelIndex]);
			CurrentLevel.Start();
			CurrentLevel.SetState(LevelState.Progress);
			OnSpawnLevel?.Invoke(CurrentLevelIndex);
		}

		private void RemoveLevel()
		{
			if (_levelContainer.TryRemove(CurrentLevel))
			{
				NavMesh.RemoveAllNavMeshData();
				OnRemoveLevel?.Invoke(CurrentLevelIndex);
			}
		}
	}
}