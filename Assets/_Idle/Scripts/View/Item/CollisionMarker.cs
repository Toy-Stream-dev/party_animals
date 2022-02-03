using System;
using System.Linq;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Item;
using _Idle.Scripts.Model.Unit;
using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.View.Item
{
	public class CollisionMarker : BaseBehaviour
	{
		public CollisionMarkerType MarkerType;
		[SerializeField] private InteractableItemType _itemType;
		public InteractableItemParams ItemParams { get; set; }
		public UnitModel UnitModel { get; set; }

		public override void StartMe()
		{
			var param = GameBalance.Instance.ItemsParams
				.FirstOrDefault(itemParams => itemParams.ItemType == _itemType);
			if (param == null)
			{
				Debug.LogError($"Can`t find ItemParams for {_itemType}");
				return;
			}
			ItemParams = param;
		}
	}
}