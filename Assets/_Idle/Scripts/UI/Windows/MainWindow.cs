using System;
using System.Collections.Generic;
using System.Linq;
using _Idle.Scripts.Ad;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Data;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.Saves;
using _Idle.Scripts.UI.Hud;
using _Idle.Scripts.UI.Message;
using _Idle.Scripts.UI.Store;
using _Idle.Scripts.UI.Windows.DailyRewards;
using _Idle.Scripts.View.Level;
using DG.Tweening;
using GeneralTools;
using GeneralTools.Model;
using GeneralTools.Tools;
using GeneralTools.UI;
using MagneticScrollView;
using MoreMountains.NiceVibrations;
using Plugins.GeneralTools.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.Windows
{
	public class MainWindow : BaseWindow
	{
		[SerializeField] private TextMeshProUGUI _soft;
		[SerializeField] private TextMeshProUGUI _hard;
		[SerializeField] private Image _playerAvatar;
		[SerializeField] private BaseButton _dailyRewardsButton;
		[SerializeField] private BaseButton _noAdOfferButton;
		[SerializeField] private TextMeshProUGUI _countdownText;
		[SerializeField] private BaseButton _playButton;
		[SerializeField] private BaseButton _nicknameButton;
		[SerializeField] private float _endScale = 5.0f;
		[SerializeField] private GameObject _buySkinLayer;
		[SerializeField] private BaseButton _buySkinButton;
		[SerializeField] private BaseButton _settingsButton;
		[SerializeField] private InputPanel _inputPanel;
		[SerializeField] private Image _levelProgress;
		[SerializeField] private TextMeshProUGUI _levelProgressText;
		
		private GameModel _gameModel;
		private AdModel _adModel;
		private MainWindowLocation _location;
		private Tween _countdownTween;
		private InAppModel _inAppModel;
        
		[SerializeField] private Transform _container;
		[SerializeField] private MagneticScrollRect _magneticScroll;
		
		private readonly List<StoreSkinCharacterItem> _storeItems = new List<StoreSkinCharacterItem>();
		
		private Item _selectedSkin;
		private StoreSkinCharacterItem _selectedItem;

		private Item[] _skins;
		
		public Action<Vector2> OnDrag = delegate {};
		
		public override void Init()
		{
			_gameModel = Models.Get<GameModel>();
			_adModel = Models.Get<AdModel>();
			
			_playButton.SetCallback(Close);
			_nicknameButton.Callback = () => GameUI.Get<NicknameEnterWindow>().Open();
			_dailyRewardsButton.Callback = () => GameUI.Get<DailyRewardsWindow>().Open();
			_buySkinButton.SetCallback(BuySkinButtonPressed);
			_settingsButton.SetCallback(OpenSettings);
			_noAdOfferButton.SetCallback(OnPressedRemoveAd);
			
			_location = MainGame.Root.GetComponentInChildren<MainWindowLocation>(true);
			_location?.Init();
			_noAdOfferButton.SetActive(!_gameModel.Flags.Has(GameFlag.AllAdsRemoved));
			_inAppModel = Models.Get<InAppModel>();
			
			_inputPanel.OnOnDrag += OnDrag;
            
			_skins = GameBalance.Instance.ItemSkins;

			// var _storeItems = _container.GetComponentsInChildren<StoreSkinCharacterItem>();
			//
			// if (_skins.Length > _storeItems.Length)
			// {
			// }
			// else
			// {
			// 	for (var i = 0; i < _skins.Length; i++)
			// 	{
			// 		_storeItems[i].Init(_skins[i]);
			// 	}
			// }
            
			if (_skins.Length > 0)
			{
				foreach (var config in _skins)
				{
					var item = Prefabs.CopyPrefab<StoreSkinCharacterItem>(_container);
					item.Init(config);
					item.OnSkinSelected += OnSkinSelected; 
					_storeItems.Add(item);
				}
			}

			for (int i = 0; i < 9; i++)
			{
				var item = Prefabs.CopyPrefab<StoreSkinCharacterItem>(_container);
				item.ShowEmpty();
				item.OnSkinSelected += OnSkinSelected;
			}

			var current = _gameModel.GetParam(GameParamType.SelectedSkin).IntValue;
			
			_selectedItem = _storeItems[current];
			
			_magneticScroll.AssignElements();
			_magneticScroll.onSelectionChange.AddListener(SelectedItemChange);
			
			_gameModel.GetParam(GameParamType.Soft).UpdatedEvent += UpdateTexts;
			_gameModel.GetParam(GameParamType.Hard).UpdatedEvent += UpdateTexts;

			_gameModel.GetProgress(GameParamType.RatingExperience).UpdatedEvent += UpdateExperience;
			_gameModel.GetProgress(GameParamType.RatingExperience).CompletedEvent += UpdateExperience;
			
			base.Init();
		}

		private void SelectedItemChange(GameObject selectedItem)
		{
			_selectedItem?.SetSelected(false);
			var item = selectedItem.GetComponent<StoreSkinCharacterItem>();
			
			if (_selectedItem.Id != item.Id)
			{
				item.SelectItem();
			}
			
			item.SetSelected(true);
			_selectedItem = item;
			
			GameSounds.Instance.PlaySound(GameSoundType.Close, 0.15f);
			MMVibrationManager.Haptic(HapticTypes.LightImpact);
		}

		private void OnSkinSelected(bool value, int skinId)
		{
			if (skinId < 0)
				value = false;
			
			_playButton.SetInteractable(value);
			
			// for (var i = 0; i < _storeItems.Count; i++)
			// {
			// 	_storeItems[i].SetSelected(i == skinId);
			// }
		}

		public override BaseUI Open()
		{
			base.Open();
			
			_gameModel.Pause();
			
			if (_location)
			{
				_location.Enable();
			}

			UpdateTexts();
			// RunCountdown();
			Redraw();
			
			var current = _gameModel.GetParam(GameParamType.SelectedSkin).IntValue;
			_magneticScroll.ScrollTo(current);
			
			GameSounds.Instance.PlayMusic(GameSoundType.Menu);

			UpdateExperience();
			
			return this;
		}

		private void UpdateExperience()
		{
			var exp = _gameModel.GetProgress(GameParamType.RatingExperience);
			var level = _gameModel.GetParam(GameParamType.CharacterLevel).IntValue;
			var levels = GameBalance.Instance.CharacterLevels;

			if (level < levels.Count - 1)
			{
				_levelProgress.fillAmount = (float)exp.ProgressValue;
				_levelProgressText.SetText($"{exp.CurrentValue}/{levels[level].Experience}");
			}
			else
			{
				if (exp.CurrentValue < levels[level].Experience)
				{
					_levelProgress.fillAmount = (float)exp.ProgressValue;
					_levelProgressText.SetText($"{exp.CurrentValue}/{levels[levels.Count - 1].Experience}");
				}
				else
				{
					_levelProgress.fillAmount = 1;
					_levelProgressText.SetText($"Max");
				}
			}
		}
		
		public void Redraw()
		{
			_buySkinLayer.SetActive(false);
			_playButton.SetInteractable(true);
			
			var current = _gameModel.GetParam(GameParamType.SelectedSkin).IntValue;
			var purchased = _gameModel.GetParam(GameParamType.SkinsCharacter).IntValue;
			
			_gameModel.GetParam(GameParamType.SkinsCharacter).SetValue(purchased | 1);
			
			for (var i = 0; i < _storeItems.Count; i++)
			{
				_storeItems[i].Redraw(i == current);
			}
			
			_playerAvatar.sprite = GameResources.Instance.GetSprite(_skins[current].Identificator);
			
			UpdateTexts();
		}

		public void UpdateTexts()
		{
			_soft.SetText($"{_gameModel.GetParam(GameParamType.Soft).Value}");
			_hard.SetText($"{_gameModel.GetParam(GameParamType.Hard).Value}");
			_nicknameButton.SetText(_gameModel.PlayerNickname);
		}

		public override void Close()
		{
			if (_location)
			{
				_location.Disable();
			}
			
			_gameModel.LevelStart();
			
			base.Close();
		}
		
		private void OnPressedRemoveAd()
		{
			var offer = GameBalance.Instance.Offers.FirstOrDefault(o => o.Id == 1);
			if (offer == null) return;
			_inAppModel.Purchase(offer.ShopId, OnPurchased, OnFailed);
		}
		
		public void OnPurchased(string token = "") //TODO: why is this?
		{
			_gameModel.Flags.Set(GameFlag.AllAdsRemoved);
			Models.Get<AdModel>().HideBanner();
			GameSave.Save();
			GetComponentInParent<SafeArea.SafeArea>().UpdateOffset();
			_noAdOfferButton.Deactivate();
			
			_playButton.rectTransform.anchoredPosition = new Vector2(0, 400); //TODO: release it
		}
        
		private void OnFailed()
		{
			GameUI.Get<MessageContainer>().Show("Purchase: FAIL");
		}

		public void ShowBuyLayer(Item item)
		{
			_selectedSkin = item;
			_buySkinLayer.Activate();

			switch (item.PurchaseType)
			{
				case PurchaseType.Soft:
					_buySkinButton.SetText($"{item.PriceAmount} <sprite name=money>");
					break;
				case PurchaseType.Ads:
					var record = _gameModel.SkinsAdsWatched.FirstOrDefault(x => x.x == _selectedSkin.Id);
					if (record == null)
					{
						record = new Int2(_selectedSkin.Id, 0);
						_gameModel.SkinsAdsWatched.Add(record);
					}
					_buySkinButton.SetText($"<sprite name=ads> {record.y}/{item.PriceAmount}");
					break;
			}
		}

		private void BuySkinButtonPressed()
		{
			switch (_selectedSkin.PurchaseType)
			{
				case PurchaseType.Soft:
					if (_selectedSkin.PriceAmount <= _gameModel.GetParam(GameParamType.Soft).IntValue)
					{
						_gameModel.PurchaseSkinCharacter(_selectedSkin);
					}
					else
					{
						GameUI.Get<MessageContainer>().Show("Not enough money.");
						GameSounds.Instance.PlaySound(GameSoundType.FailedPurchased);
					}
					break;
				case PurchaseType.Ads:
					_adModel.AdEvent += OnAdFinished;
					_adModel.ShowAd(AdPlacement.SkinStore);
					break;
			}
		}

		private void OnAdFinished(bool success)
		{
			if (!success)
				return;
			
			var item = _gameModel.SkinsAdsWatched.FirstOrDefault(x => x.x == _selectedSkin.Id);
			if (item == null)
			{
				item = new Int2(_selectedSkin.Id, 0);
				_gameModel.SkinsAdsWatched.Add(item);
			}
			
			item.y++;

			if (item.y == _selectedSkin.PriceAmount)
			{
				_gameModel.PurchaseSkinCharacter(_selectedSkin);
			}
			else
			{
				_buySkinButton.SetText($"<sprite name=ads> {item.y}/{_selectedSkin.PriceAmount}");
			}
		}

		private void OpenSettings()
		{
			GameUI.Get<SettingsWindow>().Open();
		}

		private void RunCountdown()
		{
			_countdownTween?.Kill();
			_countdownTween = null;

			var timer = 0;
			var i = 3;
			
			_countdownText.SetText(i.ToString());
			_countdownText.transform.localScale = Vector3.zero;
			_countdownTween = _countdownText.transform.DOScale(Vector3.one * _endScale, 1.0f)
				.OnStepComplete(() =>
				{
					i--;
					_countdownText.SetText(i.ToString());
					_countdownText.transform.localScale = Vector3.zero;
				})
				.SetLoops(i, LoopType.Restart)
				.OnComplete(Close);
		}
	}
}