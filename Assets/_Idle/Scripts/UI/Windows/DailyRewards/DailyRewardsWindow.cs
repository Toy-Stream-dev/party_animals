using _Idle.Scripts.Ad;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.Model.Numbers;
using _Idle.Scripts.Saves;
using _Idle.Scripts.UI.ResourceBubble;
using GeneralTools.Model;
using GeneralTools.Tools;
using GeneralTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.Windows.DailyRewards
{
    public class DailyRewardsWindow : BaseSoundableWindow
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private BaseButton _get;
        [SerializeField] private BaseButton _adButton;
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private Image _adImage;
        
        private DailyRewardItem[] _items;

        private GameModel _game;
        private AdModel _ad;
        private GameBalance _balance;
        private GameProgress _counterProgress;
        private GameProgress _timerProgress;

        private bool _watched;

        public override void Init()
        {
            _get.SetCallback(OnPressedGet);
            _adButton.SetCallback(OnPressedAd);
            
            _game = Models.Get<GameModel>();
            _ad = Models.Get<AdModel>();
            _balance = GameBalance.Instance;
            _items = GetComponentsInChildren<DailyRewardItem>();

            base.Init();
        }
        
        public override BaseUI Open()
        {
            _timer.text = "";
            
            _counterProgress = _game.GetProgress(GameParamType.DailyRewardDay);
            _counterProgress.UpdatedEvent += DayOnUpdate;
            
            _timerProgress = _game.GetProgress(GameParamType.DailyRewardTimer);
            _timerProgress.UpdatedEvent += TimerOnUpdated;

            _adImage.SetActive(!Models.Get<GameModel>().Flags.Has(GameFlag.AllAdsRemoved));
            
            Redraw();
            
            return base.Open();
        }

        private void DayOnUpdate()
        {
            Redraw();
        }

        private void TimerOnUpdated()
        {
            _timer.text = TimeAndValuesDrawer.GetTimeStr((float)(86400 - _timerProgress.CurrentValue), 3);
            _timer.SetActive(_timerProgress.CurrentValue > 0);
            _get.SetInteractable(_timerProgress.CurrentValue <= 0);
        }

        private void Redraw()
        {
            var currentDay = (int)_game.GetProgress(GameParamType.DailyRewardDay).CurrentValue;
            var items = GameBalance.Instance.DailyRewards;
            
            for (var i = 0; i < _items.Length; i++)
            {
                _items[i].Show(items[i].Day, currentDay > i, currentDay < i, items[i]);
            }

            var showAdButton = _timerProgress.CurrentValue > 0 && !_watched;
            _get.SetActive(_timerProgress.CurrentValue <= 0);
            _adButton.SetActive(showAdButton);
        }

        private void OnPressedGet()
        {
            var currentDay = (int)_game.GetProgress(GameParamType.DailyRewardDay).CurrentValue;
            var reward = GameBalance.Instance.DailyRewards[currentDay];
            _game.AddSoft(reward.Amount);
            GameUI.Get<ResourceBubblesContainer>().Show(BubbleTypes.Soft, reward.Amount, _get.rectTransform.position);
            
            _game.IncreaseDailyRewardDate();
            
            var showAdButton = _timerProgress.CurrentValue > 0 && !_watched;
            _get.SetActive(_timerProgress.CurrentValue <= 0);
            _adButton.SetActive(showAdButton);

            _watched = true;
            
            GameSounds.Instance.PlaySound(GameSoundType.GetSoftFromAd);

            GameSave.Save();
        }

        private void OnPressedAd()
        {
            _ad.AdEvent += OnAdFinished;
            _ad.ShowAd(AdPlacement.DailyBonus);
        }
        
        private void OnAdFinished(bool success)
        {
            _ad.AdEvent -= OnAdFinished;
            if (!success) return;

            var data = Models.Get<GameModel>().GetParam(GameParamType.Soft);
            data.Change(100);
            GameUI.Get<ResourceBubblesContainer>().Show(BubbleTypes.Soft, 10, _adButton.rectTransform.position);

            _adButton.Deactivate();
            
            GameSounds.Instance.PlaySound(GameSoundType.GetSoftFromAd);
        }
        
        public override void Close()
        {
            _counterProgress.UpdatedEvent -= DayOnUpdate;
            _timerProgress.UpdatedEvent -= TimerOnUpdated;
            base.Close();
        }
    }
}