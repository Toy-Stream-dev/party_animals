using System;
using _Idle.Scripts.Model;
using _Idle.Scripts.Model.Level;
using _Idle.Scripts.Model.Unit;
using _Idle.Scripts.UI.HUD;
using _Idle.Scripts.UI.Windows;
using Cinemachine;
using GeneralTools;
using GeneralTools.Model;
using GeneralTools.Tools;
using GeneralTools.UI;
using UnityEngine;

namespace _Idle.Scripts.View.Level
{
	public class MainWindowLocation : BaseBehaviour
	{
		[SerializeField] private Transform _camFollowTarget;
		[SerializeField] private float _rotateSpeedMultiplier = 1.0f;
		
		private GameModel _game;
		private Hud _hud;
		private MainWindow _mainWindow;
		private LevelLoader _level;
		private CinemachineBrain _camera;
		private CinemachineVirtualCamera _vcam;
		private Vector3 _rotation;
		private UnitModel _player;
		
		public void Init()
		{
			_game = Models.Get<GameModel>();
			_hud = GameUI.Get<Hud>();
			_mainWindow = GameUI.Get<MainWindow>();
			_camera = GameCamera.UnityCam.GetComponent<CinemachineBrain>();
			_vcam = GetComponentInChildren<CinemachineVirtualCamera>();
			
			_mainWindow.OnDrag += Rotate;
		}

		public void Enable()
		{
			_player = _game.Player;
			
			_level ??= _game.LevelLoader;
			_level.CurrentLevel.View.Deactivate();
			transform.position = _game.Player.View.transform.position;
			_camFollowTarget.localRotation = _game.Player.View.transform.localRotation;
			_vcam.enabled = true;
			_camera.enabled = false;
			var cam = _camera.transform;
			var vcam = _vcam.transform;
			cam.position = vcam.position;
			cam.rotation = vcam.rotation;
			_camera.enabled = true;
			_rotation = _camFollowTarget.localRotation.eulerAngles;
			
			this.Activate();
		}

		public void Disable()
		{
			_level ??= _game.LevelLoader;
			_level.CurrentLevel.View.Activate();
			_vcam.enabled = false;
			
			this.Deactivate();
		}

		public void Rotate(Vector2 delta)
		{
			if (gameObject.activeSelf == false) 
				return;
			
			_rotation.y += delta.x * _rotateSpeedMultiplier;
			// _camFollowTarget.localRotation = Quaternion.Euler(_rotation);
			_player.SetRotation(_rotation);
		}
	}
}