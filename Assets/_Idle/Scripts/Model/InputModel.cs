using _Idle.Scripts.Balance;
using _Idle.Scripts.Model.Components;
using _Idle.Scripts.Model.Player;
using _Idle.Scripts.Model.Unit;
using _Idle.Scripts.UI.Windows;
using GeneralTools.Model;
using GeneralTools.UI;
using UnityEngine;

namespace _Idle.Scripts.Model
{
	public class InputModel : BaseModel
	{
		private UnitModel _player;
		private Joystick _joystick;

		private Vector3 _input;
		private MoveComponent _moveComponent;

		private bool _playerAlive;
		
		private Vector2 _lastInputPoint;
		private Vector2 _inputCurrent;
		private bool _pressed;
		private float _sense;
		private GameModel _game;

		public override BaseModel Init()
		{
			_game = Models.Get<GameModel>();
			
			return base.Init();
		}

		public override BaseModel Start()
		{
			_joystick = GameUI.Get<Joystick>();
			_sense = GameBalance.Instance.TouchpadSense;
			return base.Start();
		}

		public void OnSpawnPlayer()
		{
			_player = _game.Player;
			_moveComponent = _player.Get<MoveComponent>();
			_moveComponent.OnEnd += OnMoveComponentEnd;
			_playerAlive = true;
		}

		public void OnMoveComponentEnd()
		{
			_moveComponent.OnEnd -= OnMoveComponentEnd;
			_playerAlive = false;
		}

		public override void Update(float deltaTime)
		{
			_input.x = _joystick.Direction.x;
			_input.z = _joystick.Direction.y;
			
			CheckInput();
			if (_pressed)
			{
				Pressed(deltaTime);
			}
			else
			{
				_inputCurrent = Vector2.zero;
			}

			base.Update(deltaTime);
		}

		public override void FixedUpdate(float deltaTime)
		{
			if (!_playerAlive) 
				return;
			
			_moveComponent.Move(_input, deltaTime);
			
			base.FixedUpdate(deltaTime);
		}
		
		private void CheckInput()
		{			
			if (Input.GetMouseButtonDown(0))
			{
				PointerDown();
			}
			if (Input.GetMouseButtonUp(0))
			{
				PointerUp();
			}
		}
		
		private void PointerDown()
		{
			_pressed = true;
			_lastInputPoint = Input.mousePosition;
		}
		
		private void Pressed(float deltaTime)
		{
			_inputCurrent = (Vector2)Input.mousePosition - _lastInputPoint;
			_lastInputPoint = Input.mousePosition;
			_inputCurrent *= deltaTime * _sense;
		}
		
		private void PointerUp()
		{
			_pressed = false;
		}
	}
}