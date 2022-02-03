using _Idle.Scripts.Balance;
using _Idle.Scripts.Data;
using _Idle.Scripts.UI.HUD;
using _Idle.Scripts.View.Unit;
using GeneralTools.Model;
using GeneralTools.UI;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Idle.Scripts.Model.Player
{
	public class PlayerModel : ModelWithView<PlayerView>
	{
		private PlayerData _playerData;
		private float _deltaTime;

		private GameModel _gameModel;
		private Hud _hud;
		private MMFeedbackCameraZoom _zoomFeedback;
		
		private static int Idle = Animator.StringToHash("idle"); 
		private static int Run = Animator.StringToHash("run"); 
		private static int Action = Animator.StringToHash("action"); 
		private static int Complete = Animator.StringToHash("dance");
		private static int Flip = Animator.StringToHash("flip"); 
		private static int Fall = Animator.StringToHash("falling"); 
		private static int Defeat = Animator.StringToHash("falling_impact"); 

		public UnitState CurrentState { get; private set; }


		public PlayerModel Start()
		{
			_playerData = GameBalance.Instance.PlayerData;
			_gameModel = Models.Get<GameModel>();
			_hud = GameUI.Get<Hud>();
			
			base.Start();
			return this;
		}

		public void AfterInit()
		{
		}

		public PlayerModel SpawnView(Transform root)
		{
			base.SpawnView(root);
			return this;
		}

		public override void FixedUpdate(float deltaTime)
		{
			base.FixedUpdate(deltaTime);
		}

		public override void Update(float deltaTime)
		{
			_deltaTime = deltaTime;
				
			base.Update(deltaTime);
		}

		public void SetPosition(Vector3 position)
		{
			View.SetPosition(position);
		}

		public void SetRotation(Vector3 rotation)
		{
			View.SetRotation(rotation);
		}
		
		public void Clear()
		{
		}

		public void LevelStart()
		{
		}

		public void Hook(PointerEventData data)
		{
		}

		public void Unhook()
		{
		}

		public void Boost()
		{
		}
		
		public void Rotation(Vector2 delta)
		{
		}
		
		public void OnCollisionEnter(Collision other)
		{
		}
		
		public void OnTriggerEnter(Collider other)
		{
			//TODO: release it
		}

		public void Die()
		{
		}

		public void SetState(UnitState state)
		{
			if (CurrentState == state)
				return;

			CurrentState = state;

			switch (state)
			{
				case UnitState.Die:
						Die();
					break;
			}
		}
		
		public bool Has(UnitState state)
		{
			return CurrentState == state;
		}
	}
}