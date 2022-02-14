using System;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Item;
using _Idle.Scripts.Model.Unit;
using Plugins.GeneralTools.Scripts.View;
using UnityEngine;

namespace _Idle.Scripts.View.Item
{
	public class InteractableItemView : ViewWithModel<InteractableItemModel>
	{
		[SerializeField] private CollisionMarker _collisionMarker;
		[SerializeField] private CollisionListener _interactableCollisionListener;
		public InteractableItemType ItemType;
		// public List<Collider> Collider;
		public Rigidbody ItemRigidbody;
		public Collider Collider;
		public GameObject GrabbedObject;
		public Transform GrabbedPoint1;
		public Transform GrabbedPoint2;
		public Collider MeleeWeaponCollider;
		public Collider RangeWeaponCollider;
		public Outline Outline;

		public CollisionMarker CollisionMarker => _collisionMarker;

		// [ShowIf("@this.ItemType == InteractableItemType.Snowballs || this.ItemType == InteractableItemType.Plates || this.ItemType == InteractableItemType.Tomatoes")]
		// public Projectile Projectile;

		// public InteractableItemParams _params;

		private Rigidbody _collisionMarkerRigidbody;
		private Vector3 _collisionMarkerDefaultPosition;
		private Quaternion _collisionMarkerDefaultRotation;

		public override ViewWithModel<InteractableItemModel> SetModel(InteractableItemModel model)
		{ 
			base.SetModel(model);

			return this;
		}

		public override void StartMe()
		{
			CollisionMarker?.StartMe();
			
			_interactableCollisionListener.OnOnTriggerEnter += OnTriggerEnter;
			_interactableCollisionListener.OnOnTriggerStay += OnTriggerEnter;
			_interactableCollisionListener.OnOnTriggerExit += OnTriggerExit;

			switch (ItemType)
			{
				case InteractableItemType.Lollipop:
				case InteractableItemType.Mailbox:
				case InteractableItemType.Spoon:
				case InteractableItemType.Cake:
				case InteractableItemType.Bomb:
				case InteractableItemType.Tomatoes:
					Model.Priority = 1;
					break;
				case InteractableItemType.Unit:
					Model.Priority = 100;
					break;
			}
			
			if (ItemType == InteractableItemType.Unit)
				return;
			
			_collisionMarkerRigidbody = _collisionMarker.GetComponent<Rigidbody>();
			_collisionMarkerDefaultPosition = _collisionMarker.transform.localPosition;
			_collisionMarkerDefaultRotation = _collisionMarker.transform.localRotation;
			
			ItemRigidbody.isKinematic = false;
		}

		private void OnEnable()
		{
			Model?.Clear();
		}

		private void FixedUpdate()
		{
			if (ItemType == InteractableItemType.Unit)
				return;
			
			_collisionMarker.transform.localPosition = _collisionMarkerDefaultPosition;
			_collisionMarker.transform.localRotation = _collisionMarkerDefaultRotation;
			// Debug.Log($"CollisionEnter {other.gameObject.name}");
			// var unit = other.gameObject.GetComponentInParent<UnitView>();
			// if (unit == null)
			// 	return;
			//
			// unit.Model.MeleeWeaponStrike();
		}

		public void SetVfxActive(bool active)
		{
		}

		public override void Clear()
		{
			_interactableCollisionListener.OnOnTriggerEnter -= OnTriggerEnter;
			_interactableCollisionListener.OnOnTriggerStay -= OnTriggerEnter;
			_interactableCollisionListener.OnOnTriggerExit -= OnTriggerExit;
			SetVfxActive(true);
			base.Clear();
		}

		private void OnTriggerEnter(Collider other)
		{
			Model?.TriggerEnter(other);
		}

		private void OnTriggerExit(Collider other)
		{
			Model?.OnTriggerExit(other);
		}

		private void OnCollisionEnter(Collision other)
		{
			Model?.OnCollisionEnter(other);
		}
	}
}