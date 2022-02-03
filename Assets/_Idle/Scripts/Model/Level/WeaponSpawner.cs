using System.Collections.Generic;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Item;
using _Idle.Scripts.View.Item;
using _Idle.Scripts.View.Level;
using GeneralTools.Model;
using GeneralTools.Pooling;
using GeneralTools.Tools;
using UnityEngine;

namespace _Idle.Scripts.Model.Level
{
	public class WeaponSpawner
	{
		private GameModel _gameModel;
		private List<SpawnPoint> _spawnPoints;
		private List<InteractableItemModel> _items;

		private InteractableItemType[] _weaponList = 
		{
			InteractableItemType.Lollipop,
			InteractableItemType.Mailbox,
			InteractableItemType.Spoon,
			InteractableItemType.Cake,
			InteractableItemType.Bomb,
			InteractableItemType.Tomatoes
		};
		
		public WeaponSpawner(List<SpawnPoint> spawnPoints)
		{
			_spawnPoints = spawnPoints;
			_items = new List<InteractableItemModel>();
			_gameModel = Models.Get<GameModel>();
		}

		public void Spawn()
		{
			foreach (var point in _spawnPoints)
			{
				Spawn(point);
			}
		}

		public void Spawn(SpawnPoint point)
		{
			var randomValue = _weaponList.RandomValue();
			var item = Pool.Pop<InteractableItemView>(point.transform,  true, true, x => x.ItemType == randomValue);

			var scale = Vector3.one;
			switch (item.ItemType)
			{
				case InteractableItemType.Lollipop:
				case InteractableItemType.Mailbox:
				case InteractableItemType.Spoon:
					scale = GameBalance.Instance.MeleeWeaponScale;
					break;
				case InteractableItemType.Cake:
				case InteractableItemType.Bomb:
				case InteractableItemType.Tomatoes:
					scale = GameBalance.Instance.RangeWeaponScale;
					break;
			}
			item.transform.localScale = scale;
			item.transform.localPosition = Vector3.zero;
			item.transform.rotation = Quaternion.identity;

			var model = _gameModel.ItemsContainer.CreateItem(item);
			model.SpawnPoint = point;
			model.OnDestroy += OnDestroyWeaponItem;
			_items.Add(model);
		}

		private void OnDestroyWeaponItem((InteractableItemModel, SpawnPoint) value)
		{
			_items.Remove(value.Item1);
			_gameModel.ItemsContainer.Remove(value.Item1);
			
			value.Item1.OnDestroy -= OnDestroyWeaponItem;
			Spawn(value.Item2);
		}
	}
}