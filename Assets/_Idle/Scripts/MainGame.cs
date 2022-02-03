using _Idle.Scripts.Model;
using _Idle.Scripts.Saves;
using DG.Tweening;
using GeneralTools;
using GeneralTools.Model;
using GeneralTools.UI;
using UnityEngine;
using UnityEngine.AI;

namespace _Idle.Scripts
{
	public class MainGame : BaseBehaviour
	{
		[SerializeField] private Transform _playerContainer;
		[SerializeField] private Transform _enemiesContainer;
		[SerializeField] private Transform _itemsContainer;
		[SerializeField] private Transform _environmentContainer;
		[SerializeField] private Transform _levelContainer;
		[SerializeField] private Transform _effectContainer;
		[SerializeField] private Feedbacks _feedbacks;
		
		private LoaderModel _loader;

		public static Transform Root { get; private set; }
		
		public static Transform PlayerContainer { get; private set; }
		public static Transform EnemiesContainer { get; private set; }
		
		public static Transform ItemsContainer { get; private set; }
		
		public static Transform EnvironmentContainer { get; private set; }
		public static Transform LevelContainer { get; private set; }
		public static Transform EffectContainer { get; private set; }

		public static Feedbacks Feedbacks { get; set; }

		private void Start()
		{
#if UNITY_EDITOR
			QualitySettings.vSyncCount = 0;
#else
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
#endif

			Root = transform;
			PlayerContainer = _playerContainer;
			EnemiesContainer = _enemiesContainer;
			ItemsContainer = _itemsContainer;
			EnvironmentContainer = _environmentContainer;
			LevelContainer = _levelContainer;
			EffectContainer = _effectContainer;
			
			Feedbacks = _feedbacks;

			_loader = Models.Add<LoaderModel>();
			_loader.Start();

			Application.focusChanged += OnApplicationFocusChanged;
		}

		private void Update()
		{
			var deltaTime = Time.deltaTime;

			if (_loader.State != LoaderState.Finished)
			{
				_loader.Update(deltaTime);
				return;
			}

			DOTween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
			
			GameCamera.Instance.UpdateMe(deltaTime);
			Models.Update(deltaTime);
			GameUI.UpdateMe(deltaTime);
			GameSave.Update(deltaTime);
			Cheats.Update();
		}

		private void FixedUpdate()
		{
			if (_loader.State != LoaderState.Finished)
				return;
			
			Models.FixedUpdate(Time.fixedDeltaTime);
		}

		private void LateUpdate()
		{
			if (_loader.State == LoaderState.Finished)
			{
				GameUI.LateUpdateMe();
			}
		}

		private void OnApplicationFocusChanged(bool active)
		{
#if UNITY_EDITOR
			return;
#endif
		}
	}
}