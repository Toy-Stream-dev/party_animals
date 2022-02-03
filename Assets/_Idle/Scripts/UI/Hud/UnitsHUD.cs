using System.Collections.Generic;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Model;
using _Idle.Scripts.Model.Unit;
using GeneralTools.Model;
using GeneralTools.Pooling;
using GeneralTools.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace _Idle.Scripts.UI.Hud
{
	public class UnitsHUD : BaseUIBehaviour
	{
		[ShowInInspector]
		private readonly List<UnitIconUI> _units = new List<UnitIconUI>();

		private GameModel _gameModel;
		
		public override void StartMe()
		{
			Pool.Spawn<UnitIconUI>(10);

			_gameModel = Models.Get<GameModel>();
			_gameModel.UnitsContainer.OnSpawnUnit += OnSpawnUnit;
		}

		public void OnSpawnUnit(UnitModel unit, Item skin)
		{
			var item = Pool.Pop<UnitIconUI>(transform);
			item.Init(unit, skin);
			
			_units.Add(item);
		}

		public void Clear()
		{
			foreach (var item in _units)
			{
				item.PushToPool();
			}
			
			_units.Clear();
		}
	}
}