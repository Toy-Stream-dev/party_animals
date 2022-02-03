using System;
using UnityEngine;

namespace _Idle.Scripts.Data
{
	[Serializable]
	public class PlayerData
	{
		public string Nickname = "user6803";
		[Header("Default settings")]
		public Vector3 Gravity = new Vector3(0, 0, 0);
		
		[Header("Move settings")]
		public float MoveSpeed = 1.0f;
		
		[Header("Turn settings")] 
		public float TurnSpeed = 5.0f;

		[Header("Stun settings")]
		public float StunTime;
		public float StunResetTime;
		public int StunPoints = 10;
		public int FallPoint = 5;

		[Header("Fall settings")]
		public float FallTime;

		[Header("Lift settings")]
		public float LiftTime;

		[Header("Attack settings")] 
		public float MinHandAttackImpulse = 2.0f;
		public float RangeDamageTimeout = 0.5f;
	}
}