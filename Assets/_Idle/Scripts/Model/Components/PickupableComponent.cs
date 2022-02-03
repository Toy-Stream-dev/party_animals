using System;
using System.Collections.Generic;
using _Idle.Scripts.Model.Item;
using _Idle.Scripts.Model.Numbers;
using _Idle.Scripts.Model.Unit;
using GeneralTools.Model;
using UnityEngine;

namespace _Idle.Scripts.Model.Components
{
	public class PickupableComponent : BaseComponent<UnitModel>
	{
		public event Action<BigNumber> CollectItem;
		
		public UnitModel Model => base.Model;
		
		public int ItemsCount
		{
			get => _pickupItemsCount;
			set => _pickupItemsCount = value;
		}

		private int _pickupItemsCount;
		private Stack<InteractableItemModel> _items = new Stack<InteractableItemModel>();
		// private PopupContainer _popupContainer;

		private const float OFFSET_Y = 0.054f;
		
		public PickupableComponent(UnitModel model) : base(model)
		{
			// Models.Get<GameModel>().PickupableUnits.Add(this);
			
			// _popupContainer = GameUI.Get<PopupContainer>();
		}

		public override void End()
		{
			CollectItem = null;
			// Models.Get<GameModel>().PickupableUnits.Remove(this);
		}

		public void PickUp(InteractableItemModel item)
		{
			// Models.Get<GameModel>().OnPoint(Vector2.zero);
			
			PlayPickUpVFX(item.Position);
			
			_items.Push(item);
			
			// _pickupItemsCount++;
			// _popupTextContainer.Show("+1");
			VibratorModel.Vibrate(20);
		}

		public bool TryDropItems(int dropCount, out int droppedCount)
		{
			if (_pickupItemsCount < 1)
			{
				droppedCount = 0;
				return false;
			}
			
			var currentCount = _pickupItemsCount;
			DropItems(dropCount);
			droppedCount = currentCount - _pickupItemsCount;
			
			return true;
		}

		public void DropItems(int dropCount)
		{
			return;
			dropCount = Mathf.Min(dropCount, _pickupItemsCount);
			for (var i = 0; i < dropCount; i++)
			{
				var item = _items.Pop();
				item.DropRandom();
				
				_pickupItemsCount--;
				// _popupContainer.ShowText("-1");
				VibratorModel.Vibrate(20);
			}
		}

		private void PlayPickUpVFX(Vector3 playPosition)
		{
		}

		public void Clear()
		{
			_pickupItemsCount = 0;
			_items.Clear();
		}

		~PickupableComponent()
		{
			End();
		}
	}
}