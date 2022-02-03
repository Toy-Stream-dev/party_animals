﻿using System.Collections.Generic;
using System.Linq;
using GeneralTools.View;
using UnityEngine;
using UnityEngine.AI;

namespace _Idle.Scripts.View.Level
{
	public class LevelView : BaseView
	{
		[SerializeField] private int _levelIndex;
		[SerializeField] private Transform _playerSpawnPoint;
		[SerializeField] private List<ThrowPoint> _throwPoints;

		public NavMeshSurface NavMeshSurface;

		public int LevelIndex => _levelIndex;
		public Vector3 PlayerSpawnPosition => _playerSpawnPoint.position;
		public List<ThrowPoint> ThrowPoints => _throwPoints;

		public override void Init()
		{
			_throwPoints = GetComponentsInChildren<ThrowPoint>().ToList();
			
			base.Init();
		}
	}
}