using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.GeneralTools.Scripts.View;
using RootMotion.Dynamics;
using UnityEngine;

namespace _Idle.Scripts.Model.Player
{
	public class PlayerView : ViewWithModel<PlayerModel>
	{
		[SerializeField] private Animator _animator;

		public ConfigurableJoint Joint;
		public Rigidbody Rigidbody;
		// public Transform Hips;
		public LineRenderer Rope;
		public ParticleSystem SpeedVFX;
		public ParticleSystem StunVFX;
		public PuppetMaster Puppet;
		public Collider Collider;
		
		private List<Costume> _costumes;

		private IAnimationEventsSender _animationEventsSender;
		private Action _currentCallback;

		public List<Costume> Costumes => _costumes;
		
		public override void StartMe()
		{
			_costumes = GetComponentsInChildren<Costume>().ToList();
				
			// _animationEventsSender = GetComponentInChildren<IAnimationEventsSender>();
			// _animationEventsSender?.AssignListener(this);
		}

		public void SetPosition(Vector3 position)
		{
			transform.position = position;
			// Joint.transform.position = position;
			// Hips.position = position;
		}

		public void SetRotation(Vector3 rotation)
		{
			transform.rotation = Quaternion.Euler(rotation);
		}

		public void Animate(int animationHashKey, Action callback = null)
		{
			_currentCallback = callback;
			_animator.CrossFadeInFixedTime(animationHashKey, 0.1f);
		}

		public void ExecuteEvent()
		{
			_currentCallback?.Invoke();
		}

		private void OnCollisionEnter(Collision other)
		{
			Model.OnCollisionEnter(other);
		}

		private void OnTriggerEnter(Collider other)
		{
			Model.OnTriggerEnter(other);
		}

		private void OnAnimatorIK(int layerIndex)
		{
			// if (!Model.Hooked)
			// 	return;
			//
			// _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
			// _animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.identity);;
		}
	}
}