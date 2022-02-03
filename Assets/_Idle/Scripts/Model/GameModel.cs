using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using _Idle.Scripts.Ad;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Base;
using _Idle.Scripts.Model.Level;
using _Idle.Scripts.Model.Numbers;
using _Idle.Scripts.Model.Player;
using _Idle.Scripts.Model.Unit;
using _Idle.Scripts.Saves;
using _Idle.Scripts.UI;
using _Idle.Scripts.UI.HUD;
using _Idle.Scripts.UI.Windows;
using _Idle.Scripts.View.Level;
using _Idle.Scripts.View.Unit;
using DG.Tweening;
using GeneralTools;
using GeneralTools.Model;
using GeneralTools.Tools;
using GeneralTools.UI;
using UnityEngine;
using Random = GeneralTools.Tools.Random;

namespace _Idle.Scripts.Model
{
	public class GameModel : GameModelBase, IModelWithParam
	{
		public event Action SoftUpdatedEvent;

		private BadgesModel _badges;
		private Hud _hud;
		
		private GameParam _totalPlayTime;
		private GameProgress _dailyRewardTimer;
		
		private PlayerContainer _playerContainer; 
		private UnitContainer _unitContainer; 
		private ItemsContainer _itemsContainer; 
		
		private GameProgress _progress;
		private LoadingWindow _loader;
		private AdModel _ad;

		private bool _paused;
		private bool _runningInBackground;
		private DateTime _firstSessionStart;
		private RaycastHit[] hitInfo = new RaycastHit[1];
		private Vector3 _lastPlayerPosition;
		private TimeScale _timeScale;
		
		private int _fakeLevelIndex;

		private MainWindowLocation _mainLocation;
		private string[] _nicknames;
		private string[] _generatedNames;
		
		[SerializeField] private string _firstSessionStartStr;
		

		public UnitModel Player { get; set; }
		
		public LevelLoader LevelLoader { get; private set; }
		public bool Paused => _paused;

		public ItemsContainer ItemsContainer => _itemsContainer;
		public PlayerContainer PlayerContainer => _playerContainer;
		public UnitContainer UnitsContainer => _unitContainer;
		public TimeScale TimeScale => _timeScale;
		
		public override BaseModel Init()
		{
			base.Init();
			
			GameSounds.Instance.Init();
			
			CreateParam(GameParamType.Soft);
			CreateParam(GameParamType.Hard);
			CreateParam(GameParamType.CollectedCoins);
			CreateParam(GameParamType.SessionN);

			_playerContainer = new PlayerContainer();
			_unitContainer = new UnitContainer();
			_itemsContainer = new ItemsContainer();
			_timeScale = new TimeScale();
			
			var charLevel = CreateParam(GameParamType.CharacterLevel);
			var levels = GameBalance.Instance.CharacterLevels;
			int nextLevelExp;
			
			if (charLevel.IntValue < levels.Count - 1)
			{
				nextLevelExp = levels[charLevel.IntValue].Experience;
			}
			else
			{
				nextLevelExp = levels[levels.Count - 1].Experience;
			}
			var expProgress = CreateProgress(GameParamType.RatingExperience, nextLevelExp, false);
			expProgress.CompletedEvent += UpdateNeededExperience;
			
			var dailyReward = CreateProgress(GameParamType.DailyRewardDay, GameBalance.Instance.DailyRewards.Count, false);
			dailyReward.CompletedEvent += DailyRewardOnComplete;
			dailyReward.SetValue(0);
			
			_dailyRewardTimer = CreateProgress(GameParamType.DailyRewardTimer, 86400, false);
			_dailyRewardTimer.Pause();
			_dailyRewardTimer.CompletedEvent += DailyRewardTimerOnCompleted;
			
			InitGameData();
			_totalPlayTime = CreateParam(GameParamType.TotalPlayTime, 0, false);
			
			_progress = CreateProgress(GameParamType.Progress, 10, false);
			// _progress.CompletedEvent += LevelComplete;
			if (GetParam(GameParamType.RatingPlace).IntValue == 0)
			{
				GetParam(GameParamType.RatingPlace).SetValue(GameBalance.Instance.StartRatingPlace);
			}

			_mainLocation = MainGame.Root.GetComponentInChildren<MainWindowLocation>(true);
			if (!_mainLocation)
			{
				_mainLocation = Prefabs.CopyPrefab<MainWindowLocation>(MainGame.Root);
			}
			return this;
		}

		public override BaseModel Start()
		{
			base.Start();

			Flags.ChangedEvent += OnFlagChanged;
			
			_badges = Models.Get<BadgesModel>();
			_ad = Models.Get<AdModel>();
			_loader = GameUI.Get<LoadingWindow>();
			_hud = GameUI.Get<Hud>();
			_hud.ShowLevel(1, 2);
			
			_fakeLevelIndex = Mathf.Max(GetParam(GameParamType.CurrentLevel).Level, 1);
			
			var session = GetParam(GameParamType.SessionN);
			if (session.IntValue == 0)
			{
				AppEventsProvider.TriggerEvent(GameEvents.FirstSession);
				session.Change(1);
			}
			
			_unitContainer.Start();
			_playerContainer.Start();
			
			InitPlayer();
			CreateLevelLoader();
			
			return this;
		}
		
		public override void Update(float deltaTime)
		{
			if (_paused)
				return;
			_dailyRewardTimer?.Change(deltaTime);
			IncPlayTime(deltaTime);
			_playerContainer.Update(deltaTime);
			_unitContainer.Update(deltaTime);
			_itemsContainer.Update(deltaTime);
			
			LevelLoader.Update(deltaTime);

			//_progress.SetValue(Player.Position.z);
			
			base.Update(deltaTime);
		}

		public override void FixedUpdate(float deltaTime)
		{
			if (_paused)
				return;
			
			_playerContainer.FixedUpdate(deltaTime);
			_unitContainer.FixedUpdate(deltaTime);
			
			base.FixedUpdate(deltaTime);
		}

		public void OnLoaded()
		{
			var skins = GetParam(GameParamType.SkinsCharacter).IntValue;
			GetParam(GameParamType.SkinsCharacter).SetValue(skins | 1);
			
			_badges.Update();
			_hud.Open();
			
			if (GetParam(GameParamType.RatingPlace).IntValue == 0)
			{
				GetParam(GameParamType.RatingPlace).SetValue(GameBalance.Instance.StartRatingPlace);
			}
			
			AppEventsProvider.TriggerEvent(GameEvents.GameStart);
			
			_ad.ShowAd(AdPlacement.Banner, AdVideoType.Banner);
		}
		
		private void IncPlayTime(float deltaTime)
		{
			var prevMinute = (int)_totalPlayTime.Value / 60;
			_totalPlayTime.Change(deltaTime);
			var minutes = (int)_totalPlayTime.Value / 60;

			if (prevMinute != minutes && minutes <= 60)
			{
				AppEventsProvider.TriggerEvent(GameEvents.Timer, minutes, GetParam(GameParamType.SessionN).IntValue);
			}
		}
		
		public int GetBadgeCount(BadgeNotificationType type) => _badges.GetCount(type);

		public void CopyFrom(GameModel source)
		{
			base.CopyFrom(source);
			
			_firstSessionStart = source._firstSessionStart;
		}

		public override void OnBeforeSerialize()
		{
			_firstSessionStartStr = _firstSessionStart.ToString(CultureInfo.InvariantCulture);
			base.OnBeforeSerialize();
		}

		public override void OnAfterDeserialize()
		{
			_firstSessionStart = DateTime.Parse(_firstSessionStartStr, CultureInfo.InvariantCulture);
			base.OnAfterDeserialize();
		}

		public void Pause()
		{
			_paused = true;
		}

		public void Unpause()
		{
			_paused = false;
		}

		public void ShowStartOverlay()
		{
			GameUI.Get<MainWindow>().Open();
		}

		public void LoadNextLevel()
		{
			_unitContainer.DestroyAll();
			_itemsContainer.DestroyAll(true);
			if (Player == null)
			{
				InitPlayer();	
			}
			// _loader.EmulateLoading(GameBalance.Instance.EmulateLoadingTime);

			if (!LevelLoader.CurrentLevel.LevelParams.IsTutorial)
			{
				_ad.ShowAd(AdPlacement.LevelComplete, AdVideoType.Interstitial);
			}
			
			LevelLoader.LoadNextLevel();
		}

		public void RestartLevel()
		{
			Unpause();
			
			_unitContainer.DestroyAll();
			_itemsContainer.DestroyAll(true);
			if (Player == null)
			{
				InitPlayer();	
			}
			
			// _loader.EmulateLoading(GameBalance.Instance.EmulateLoadingTime);
			LevelLoader.LoadLevel(_fakeLevelIndex);
			
			_hud.ClearUnits();
			_hud.Activate();
			
			AppEventsProvider.TriggerEvent(GameEvents.LevelFail, _fakeLevelIndex);
		}

		//TODO: release revive
		public void RevivePlayer()
		{
			Unpause();
			
			if (Player == null)
			{
				InitPlayer();
			}
			
			Player.SetPosition(LevelLoader.CurrentLevel.View.PlayerSpawnPosition);
			Player.ResetPosition();
			
			_hud.Activate();
			_hud.ShowGameplayElements(GamePlayElement.SoftCurrency);
			LevelLoader.CurrentLevel.SetState(LevelState.Progress);
		}

		private void CreateLevelLoader()
		{
			LevelLoader = new LevelLoader();
			LevelLoader.OnSpawnLevel += OnSpawnLevel;
			LevelLoader.OnRemoveLevel += OnRemoveLevel;
			LevelLoader.Init();
			LevelLoader.LoadLevel(_fakeLevelIndex);
		}

		private void OnSpawnLevel(int levelIndex)
		{
			var levelParams = GameBalance.Instance.LevelParams.First(p => p.LevelIndex.Equals(levelIndex));
			_progress.Reset();
			_progress.SetTargetValue(100); //TODO:
			
			// _itemsContainer.FindExistingItems(LevelLoader.CurrentLevel.View.transform);
			
			GetParam(GameParamType.CollectedCoins).SetValue(0);

			Player.SetPosition(LevelLoader.CurrentLevel.View.PlayerSpawnPosition);
			GameCamera.Instance.Follow(Player.View.transform, false);

			_hud.ShowLevel(_fakeLevelIndex, _fakeLevelIndex + 1);
			_hud.RefreshProgress();
			
			Player.ResetPosition();
			ShowStartOverlay();
		}

		private void OnRemoveLevel(int levelIndex)
		{;
			Models.Get<GameEffectModel>().StopAll();
		}

		private void InitPlayer()
		{
			Player = _playerContainer.SpawnPlayer();
			Player.AfterInit();

			var selectedSkinId = GetParam(GameParamType.SelectedSkin).IntValue;
			Player.View.SetSkin(selectedSkinId);
			
			GameCamera.Instance.Follow(Player.View.transform, false);
			Models.Get<InputModel>().OnSpawnPlayer();
		}

		private void OnFlagChanged(GameFlag flag, bool active)
		{
			switch (flag)
			{
			}
		}

		public void LevelStart()
		{
			Unpause();
			Player.LevelStart();
			_hud.ShowLevelProgress();
			_hud.ShowGameplayElements(GamePlayElement.SoftCurrency);

			var levelParams = LevelLoader.CurrentLevel.LevelParams;
			
			if (levelParams.IsTutorial)
			{
				// var tutorial = GameUI.Get<TutorialOverlay>();
				// tutorial.UpdateText(levelParams.TutorialText);
				// tutorial.Open();
			}
			else
			{
				_unitContainer.SpawnUnits();
			}
			
			GameSounds.Instance.PlayMusic(GameSoundType.Battle);
			
			AppEventsProvider.TriggerEvent(GameEvents.LevelStart, _fakeLevelIndex);
		}

		public void LevelComplete()
		{
			if (!LevelLoader.CurrentLevel.InProgress)
				return;

			_hud.ClearUnits();
			_hud.HideGameplayElements(GamePlayElement.SoftCurrency);
			
			DOTween.Sequence().AppendInterval(GameBalance.Instance.EndLevelDelay).OnComplete(() =>
			{
				LevelLoader.CurrentLevel.SetState(LevelState.Completed);
			
				// Player.LevelComplete();
			
				AppEventsProvider.TriggerEvent(GameEvents.LevelComplete, _fakeLevelIndex);
			
				_fakeLevelIndex++;
				_hud.HideLevelProgress();
				GameUI.Get<LevelCompletedWindow>().Open();
				GetParam(GameParamType.CurrentLevel).SetLevel(_fakeLevelIndex);
				GameCamera.Instance.Follow(null);
				// Pause();
			});
		}

		public void LevelFailed()
		{
			if (!LevelLoader.CurrentLevel.InProgress)
				return;

			_hud.Deactivate();
			_hud.HideGameplayElements(GamePlayElement.SoftCurrency);

			DOTween.Sequence().AppendInterval(GameBalance.Instance.EndLevelDelay).OnComplete(() =>
			{
				LevelLoader.CurrentLevel.SetState(LevelState.Failed);
			
				Pause();
				_hud.HideLevelProgress();
				GameUI.Get<LevelFailedWindow>().Open();
			});
		}
		
		public string[] GenerateNickNames(int count)
		{
			if (_nicknames == null || _nicknames.Length == 0)
			{
				_nicknames = GameBalance.Instance.NickNames;
			}
            
			_generatedNames = new string[count];
            
			for (int i = 0; i < _generatedNames.Length; i++)
			{
				var r = Random.Range(0, _nicknames.Length);
				(_nicknames[r], _nicknames[i]) = (_nicknames[i], _nicknames[r]);
				_generatedNames[i] = _nicknames[i];
				var randText = Random.Range(0, 3);
				switch (randText)
				{
					case 0:
						_generatedNames[i] = _generatedNames[i].ToLower();
						break;
					case 2:
						_generatedNames[i] = _generatedNames[i].ToUpper();
						break;
				}
				var randNumber = Random.Range(0, 1000);
				randText = Random.Range(0, 2);

				if(randText == 1){
					_generatedNames[i] += randNumber;
				}
			}

			return _generatedNames;
		}
		
		public void PurchaseSkinCharacter(Balance.Item item)
		{
			var purchased = GetParam(GameParamType.SkinsCharacter).IntValue;
			GetParam(GameParamType.SkinsCharacter).SetValue(purchased | 1 << item.Id);
			SpendSoft(item.SoftPrice);
			SelectSkinCharacter(item);
			
			GameSounds.Instance.PlaySound(GameSoundType.SuccessPurchased);
			
			GameSave.Save();
		}

		public void SelectSkinCharacter(Balance.Item skin)
		{
			var purchased = GetParam(GameParamType.SkinsCharacter).IntValue;
			if ((purchased & 1 << skin.Id) != 0)
			{
				GetParam(GameParamType.SelectedSkin).SetValue(skin.Id);
				Player.View.SetSkin(skin.Id);

				GameSave.Save();
			}
			
			GameUI.Get<MainWindow>().Redraw();
		}
		
		public void AddSoft(int value)
		{
			if (value < 0) 
				return;
			
			GetParam(GameParamType.Soft).Change(value);
		}
		
		public void AddExp(int value)
		{
			GetProgress(GameParamType.RatingExperience).Change(value);
		}

		private void SpendSoft(int value)
		{
			if (value < 0) 
				return;
			
			GetParam(GameParamType.Soft).Change(-value);
		}

		public void SetPlayerNickname(string value, int price)
		{
			GameBalance.Instance.PlayerData.Nickname = value;
			SpendSoft(price);
			
			GameSave.Save();
		}
		
		public void IncreaseDailyRewardDate()
		{
			var param = GetProgress(GameParamType.DailyRewardDay);
			param.Change(1);
			_dailyRewardTimer.SetValue(0.1f);
			_dailyRewardTimer.Play();
		}
		
		private void DailyRewardOnComplete()
		{
			_dailyRewardTimer.SetValue(0.1f);
			_dailyRewardTimer.Play();
			
			var param = GetProgress(GameParamType.DailyRewardDay);
			param.Reset();
		}
		
		private void DailyRewardTimerOnCompleted()
		{
			var param = GetParam(GameParamType.DailyRewardDay);
			param.SetValue(GameBalance.Instance.DailyRewards.Count);
			_dailyRewardTimer.Reset();
			_dailyRewardTimer.Pause();
		}

		private void UpdateNeededExperience()
		{
			var expProgress = GetProgress(GameParamType.RatingExperience);
			expProgress.Reset();
			
			var charLevel = GetParam(GameParamType.CharacterLevel);
			charLevel.Change(1);
			
			var levels = GameBalance.Instance.CharacterLevels;
			int nextLevelExp;
			
			if (charLevel.IntValue < levels.Count - 1)
			{
				nextLevelExp = levels[charLevel.IntValue].Experience;
			}
			else
			{
				nextLevelExp = levels[levels.Count - 1].Experience;
			}
				
			Debug.Log($"{charLevel.IntValue} | {nextLevelExp}");
			
			expProgress.SetTargetValue(nextLevelExp);
		}
	}
}