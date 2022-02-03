using System;
using System.Linq;
using _Idle.Scripts.Enums;
using _Idle.Scripts.View.Level;
using GeneralTools.Model;
using UnityEngine;
using UnityEngine.AI;

namespace _Idle.Scripts.Model.Level
{
	public class LevelModel : ModelWithView<LevelView>
	{
		public LevelState CurrentState { get; private set; }
		public bool InProgress => CurrentState == LevelState.Progress;
		public bool IsCompleted => CurrentState == LevelState.Completed;
		public bool IsFailed => CurrentState == LevelState.Failed;

		private GameModel _gameModel;
		private int _currentPlatformIndex = 0;
		
		private LevelParams _levelParams;
		private float _pointDetectionDistance;
		
		// private List<SpawnPoint> _weaponSpawnPoints = new List<SpawnPoint>();
		private WeaponSpawner _weaponSpawner;

		public LevelParams LevelParams => _levelParams;
		
		public void SetParams(LevelParams levelParams)
		{
			_levelParams = levelParams;
		}
		
		public new LevelModel Start()
		{
			_gameModel = Models.Get<GameModel>();
			
			NavMesh.AddNavMeshData(View.NavMeshSurface.navMeshData);
			
			if (!_levelParams.IsTutorial)
			{
				SpawnWeapons();
			}
			
			base.Start();
			return this;
		}

		public LevelModel SpawnView(Transform root, bool activate = true, Predicate<LevelView> predicate = default)
		{
			base.SpawnView(root, true, activate, predicate);
			
			return this;
		}

		public override ModelWithView<LevelView> DestroyView()
		{
			View?.Clear();
			GameObject.Destroy(View.gameObject);
			
			return this;
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
		}

		public void SetState(LevelState state)
		{
			if (CurrentState == state)
				return;

			CurrentState = state;

			switch (state)
			{
				case LevelState.Completed:
					break;
			}
		}

		public void SpawnWeapons()
		{
			var weaponSpawnPoints = Transform.GetComponentsInChildren<SpawnPoint>().Where(x => x.PointType == SpawnPointType.Weapon).ToList();
			_weaponSpawner = new WeaponSpawner(weaponSpawnPoints);
			_weaponSpawner.Spawn();
		}

		public void SpawnWeaponItem(SpawnPoint point)
		{
			_weaponSpawner.Spawn(point);
		}
	}
}