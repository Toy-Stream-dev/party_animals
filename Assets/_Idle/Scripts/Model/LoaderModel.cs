using System;
using System.Collections.Generic;
using _Idle.Scripts.Ad;
using _Idle.Scripts.Analytics;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Saves;
using _Idle.Scripts.UI.Windows;
using GeneralTools;
using GeneralTools.Localization;
using GeneralTools.Model;
using GeneralTools.Pooling;
using GeneralTools.UI;
using UnityEngine;

namespace _Idle.Scripts.Model
{
	public enum LoaderState
	{
		None,
		Started,
		InitializingComponents,
		InitializingGameServices,
		LoadingSave,
		Finishing,
		Finished
	}

	public class LoaderModel : BaseModel
	{
		private readonly Dictionary<LoaderState, float> _stateWeights = new Dictionary<LoaderState, float>()
		{
			{LoaderState.Started, 0.05f},
			{LoaderState.InitializingComponents, 0.15f},
			{LoaderState.InitializingGameServices, 0.1f},
			{LoaderState.LoadingSave, 0.45f},
			{LoaderState.Finishing, 0.25f}
		};

		public event Action<LoaderState> StateChangedEvent;
		public event Action LoadedEvent;

		private LoadingWindow _loadingWindow;
		private List<Action> _loadingActions = new List<Action>();
		private float _loadingActionWeight;
		private float Progress { get; set; }

		public LoaderState State { get; private set; }

		public override BaseModel Start()
		{
			if (State != LoaderState.None) return this;
			StartLoading();
			return base.Start();
		}

		public void StartLoading()
		{
			Progress = _stateWeights[LoaderState.Started];

			Prefabs.Init();
			GameSettings.Load();

			OpenLoading();

			State = LoaderState.Started;
		}

		private void OpenLoading()
		{
			// _loadingWindow = GameUI.Get<LoadingWindow>();
			// _loadingWindow.Open(Progress);
			// _loadingWindow.SetLoadingText(LocalizationsContainer.Get("loading", GameSettings.Instance.Language));
		}

		private void OnLoadingStateCompleted()
		{
			State = ++State;
			StateChangedEvent?.Invoke(State);

			switch (State)
			{
				case LoaderState.InitializingComponents:
					StartInitializingComponents();
					break;

				case LoaderState.InitializingGameServices:
					StartInitializingGameServices();
					break;

				case LoaderState.LoadingSave:
					StartLoadingSave();
					break;

				case LoaderState.Finishing:
					StartFinishing();
					break;

				case LoaderState.Finished:
					OnLoaded();
					break;
			}

			_loadingActionWeight = _stateWeights.ContainsKey(State)
				                       ? _loadingActions.Count == 0 ? _stateWeights[State] :
				                                                      _stateWeights[State] / _loadingActions.Count
				                       : 0f;
		}

		public override void Update(float deltaTime)
		{
			if (State == LoaderState.None ||
			    State == LoaderState.Finished ||
			    State == LoaderState.InitializingGameServices) return;

			if (_loadingActions.Count == 0)
			{
				OnLoadingStateCompleted();
				return;
			}

			_loadingActions[0]?.Invoke();
			_loadingActions.RemoveAt(0);

			Progress += _loadingActionWeight;
			// _loadingWindow.UpdateProgress(Progress);
		}

		private void StartInitializingComponents()
		{
			_loadingActions = new List<Action>
			{
				GameBalance.Init,
				InitPool,
				CreateModels,
				InitLocalization,
				// GameSounds.Instance.Init
			};
		}

		private void InitLocalization()
		{
			GameSettings.Instance.LanguageChangedEvent += ApplyLanguage;
			Localization.LanguageChangedEvent += GameUI.UpdateLocalization;

			ApplyLanguage();

			void ApplyLanguage() => Localization.ApplyLanguage(GameSettings.Instance.Language);
		}

		private void CreateModels()
		{
			Models.Add<GameModel>();
			Models.Add<InAppModel>();
			Models.Add<InputModel>();
			Models.Add<GameEffectModel>();
			Models.Add<AdModel>();
			Models.Add<AnalyticsModel>();
			Models.Add<BadgesModel>();
		}

		private void InitPool()
		{
			Pool.Init(null);
		}

		private void StartInitializingGameServices()
		{
			OnLoadingStateCompleted();
		}

		private void StartLoadingSave()
		{
			_loadingActions = new List<Action>
			{
				LoadSave
			};
		}

		private void StartFinishing()
		{
			_loadingActions = new List<Action>
			{
				GameUI.Init,
				GameSave.Save,
				FrameDelay,
				Models.Start,
				FrameDelay
			};
		}

		private void LoadSave()
		{
			if (!GameSave.Exists() || DevFlags.RESET) return;

			var snapshot = GameSave.Load();
			if (snapshot.Game?.Flags?.Has(GameFlag.TutorialFinished) == false) return;
			Models.Get<GameModel>().CopyFrom(snapshot.Game);
		}

		private void FrameDelay()
		{
		}

		private void OnLoaded()
		{
			GameCamera.Instance.Init();
			Models.Get<GameModel>().OnLoaded();

			// _loadingWindow.Close();
			LoadedEvent?.Invoke();
		}
	}
}