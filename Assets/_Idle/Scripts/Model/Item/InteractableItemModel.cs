using System;
using System.Collections.Generic;
using System.Linq;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Components;
using _Idle.Scripts.Model.Unit;
using _Idle.Scripts.View.Item;
using _Idle.Scripts.View.Level;
using _Idle.Scripts.View.Unit;
using DG.Tweening;
using GeneralTools.Model;
using GeneralTools.Pooling;
using GeneralTools.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Idle.Scripts.Model.Item
{
	public class InteractableItemModel : ModelWithView<InteractableItemView>
	{
		private List<PickupableComponent> _pickupableUnits;
		// private Dictionary<PickupableComponent, float> _pickupUnitsTime;
		private InteractableItemParams _params;
		private int _ammoLeft;
		private int _durability = 200;
		private UnitModel _tempUnit;
		private UnitModel _owner;

		private int _interactableCount = 0;
		
		private Sequence _unitFallTween;

		//public UnitModel Owner { get; set; }
		public InteractableItemParams Params => _params;

		public int Priority; 

		public UnitModel Owner
		{
			get => _owner;
			set
			{
				_owner = value;
				SetOwner?.Invoke();
			}
		}

		public SpawnPoint SpawnPoint { get; set; }

		[ShowInInspector, ReadOnly]
		public bool IsInterctable { get; set; }

		public event Action SetOwner;
		public event Action OnDrop;
		public event Action<(InteractableItemModel, SpawnPoint)> OnDestroy;

		[ShowInInspector, ReadOnly]
		private List<int> _interactableUnits = new List<int>();

		public void TriggerEnter(Collider other)
		{
			if (!other.TryGetComponent<UnitView>(out var unit))
				return;

			switch (unit.Model.CurrentState)
			{
				case UnitState.Move:
					if (unit.UnitType == UnitType.Player)
					{
						View.Outline?.EnableOutline();
					}
					return;
				case UnitState.Finish:
				case UnitState.Lift:
				case UnitState.Lifted:
				case UnitState.Fall:
				case UnitState.DropUnit:
				case UnitState.Die:
				case UnitState.Stunned:
				case UnitState.Interactable:
					return;
			}
			
			if (_interactableUnits.Count > 0 && IsInterctable)
			{
				return;
			}
			
			if (_interactableUnits.Contains(unit.Model.UnitID))
				return;


			if (!unit.Model.HasItemPriority(this))
			{
				return;
			}
			
			switch (View.ItemType)
			{
				case InteractableItemType.Unit:
					if (!unit.Model.AblePickUpUnit)
					{
						return;
					}
					
					unit.Model.CurrentItem?.DropRandom();
					
					_owner.View.SpineCollider.Deactivate();
					break;
			}

			//_tempUnit = unit.Model;
			unit.Model.StartInteractable(this, Params);
			_interactableUnits.Add(unit.Model.UnitID);

			/*
			for (int i = 0, count = _pickupableUnits.Count; i < count; i++)
			{
				if (_pickupableUnits[i].Model.View.transform != other.transform) 
					continue;
				
				_pickupUnitsTime[_pickupableUnits[i]] = GameBalance.Instance.InteractionTime;
				_pickupableUnits[i].Model.InSearchZone = true;

				if (_pickupableUnits[i].Model.UnitType != UnitType.Player) 
					continue;
				
				_pickupableUnits[i].Model.View.Progress.SetFullTime(GameBalance.Instance.InteractionTime);
				_pickupableUnits[i].Model.View.Progress.Show();
			}
			*/

		}

		public void OnTriggerExit(Collider other)
		{
			if (!other.TryGetComponent<UnitView>(out var unit))
				return;

			OnTriggerExit(unit);
		}

		public void OnTriggerExit(UnitView unit)
		{
			if (unit.UnitType == UnitType.Player)
			{
				View.Outline?.DisableOutline();
			}
			
			if (_interactableUnits.Count <= 0)
				return;
			
			if (!_interactableUnits.Contains(unit.Model.UnitID))
				return;

			/*switch (View.ItemType)
			{
				case InteractableItemType.Unit:
					if (_owner.CurrentState == UnitState.Fall)
					{
						_unitFallTween.OnComplete(() =>
						{
							_owner.View.SpineCollider.Activate();
							
						}).Play();
					}
					break;
			}*/
			
			unit.Model.StopInteractable();
			_interactableUnits.Remove(unit.Model.UnitID);
			
			if (_interactableUnits.Count == 0)
			{
				IsInterctable = false;
			}
		}

		public void DropRandom()
		{
			var x = Random.Range(-1.0f, 2.0f);
			var y = Random.Range(3.0f, 5.0f);
			var z = Random.Range(-1.0f, 2.0f);

			Drop(new Vector3(x, y, z));
			// DOTween.Sequence().AppendInterval(0.5f).OnComplete(() =>
			// {
			// View.Collider.enabled = true;
			// View.Collider.isTrigger = false;
			// });
		}

		public void Drop(Vector3 dropDirection)
		{
			OnDrop?.Invoke();
			
			Transform.SetParent(null);
			Clear();
			
			var multiplier = GameBalance.Instance.MeleeWeaponBreakForceMultiplier;
			switch (View.ItemType)
			{
				case InteractableItemType.Cake:
				case InteractableItemType.Bomb:
				case InteractableItemType.Tomatoes:
					multiplier = GameBalance.Instance.RangeWeaponBreakForceMultiplier;
					break;
			}
			
			var rb = View.ItemRigidbody;
			rb.isKinematic = false;
			rb.AddForce(dropDirection * multiplier, ForceMode.Impulse);

			View.Collider.enabled = false;
		}

		public void Drop(Vector3 dropDirection, Vector3 forcePoint)
		{
			OnDrop?.Invoke();
			
			Transform.SetParent(null);
			Clear();
			
			var multiplier = GameBalance.Instance.MeleeWeaponBreakForceMultiplier;
			switch (View.ItemType)
			{
				case InteractableItemType.Cake:
				case InteractableItemType.Bomb:
				case InteractableItemType.Tomatoes:
					multiplier = GameBalance.Instance.RangeWeaponBreakForceMultiplier;
					break;
			}
			
			var rb = View.ItemRigidbody;
			rb.isKinematic = false;
			rb.AddForceAtPosition(dropDirection * multiplier, forcePoint, ForceMode.Impulse);

			View.Collider.enabled = false;
		}

		public void Throw(Vector3 position, Vector3 direction)
		{
			var proj = Pool.Pop<Projectile>(null, true, true, x => x.ProjectileType == View.ItemType); //TODO: change from Pool
			proj.transform.localScale = GameBalance.Instance.RangeWeaponScale;
			proj.transform.position = position;
			direction.y += 0.1f;
			proj.StartMe();
			proj.Throw(direction, _params.PunchPower);

			_ammoLeft--;

			if (_ammoLeft <= 0)
			{
				DropRandom();
				Destroy(true);
			}
		}

		public override BaseModel Init()
		{
			return base.Init();
		}
		
		

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
		}

		public InteractableItemModel Start()
		{
			_params = GameBalance.Instance.ItemsParams.First(x => x.ItemType == View.ItemType);
			
			if (_params != null && _params.IsWeapon && _params.WeaponType == WeaponType.Range)
			{
				_ammoLeft = _params.AmmoAmount;
			}

			_unitFallTween = DOTween.Sequence().AppendInterval(0.5f);
			
			base.Start();
			return this;
		}
		
		public InteractableItemModel SpawnView(Transform root, bool activate = true, Predicate<InteractableItemView> predicate = default)
		{
			base.SpawnView(root, true, activate, predicate);
			return this;
		}

		public override ModelWithView<InteractableItemView> DestroyView()
		{
			Clear();
			return base.DestroyView();
		}

		public void Clear()
		{
			_interactableUnits.Clear();
			IsInterctable = false;
		}

		public void AppendMeleeStrike()
		{
			DisableMeleeWeaponCollider();
			DropRandom();
		}

		public void EnableMeleeWeaponCollider()
		{
			View.MeleeWeaponCollider.enabled = true;
		}

		public void DisableMeleeWeaponCollider()
		{
			View.MeleeWeaponCollider.enabled = false;
		}

		public void EnableRangeWeaponCollider()
		{
			View.RangeWeaponCollider.enabled = true;
		}

		public void DisableRangeWeaponCollider()
		{
			View.RangeWeaponCollider.enabled = false;
		}

		public void OnCollisionEnter(Collision other)
		{
			if (((1 << other.gameObject.layer) & GameBalance.Instance.GroundLayer) != 0)
			{
				// if (_durability == 0)
				// {
				// 	Destroy(true);
				//
				// 	return;
				// }
				
				// Transform.gameObject.GetComponent<Rigidbody>()?.DestroyComponent();//TODO: release it
				// View.Collider.isTrigger = true;
				View.Collider.enabled = true;
			}
		}

		public void StopPickUp()
		{
			_tempUnit?.StopInteractable();
		}

		public void Destroy(bool spawnNew = false)
		{
			if (spawnNew)
			{
				OnDestroy?.Invoke((this, SpawnPoint));
			}
			
			DestroyView();
		}

		public void PickedUp()
		{
			Clear();
			View.Outline?.DisableOutline();
		}
	}
}