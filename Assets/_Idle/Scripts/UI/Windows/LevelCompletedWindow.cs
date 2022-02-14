using _Idle.Scripts.Ad;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using DG.Tweening;
using GeneralTools.Model;
using GeneralTools.Pooling;
using GeneralTools.Tools;
using GeneralTools.UI;
using GeneralTools.UI.Animations;
using MoreMountains.NiceVibrations;
using Plugins.GeneralTools.Scripts.UI;
using TMPro;
using UnityEngine;

namespace _Idle.Scripts.UI.Windows
{
    public class LevelCompletedWindow : BaseWindow
    {
        [SerializeField] private TextMeshProUGUI _soft;
        [SerializeField] private TextMeshProUGUI _earnedTextCopy;
        [SerializeField] private TextMeshProUGUI _gemPriceText;
        [SerializeField] private BaseButton _nextLevelButton;
        [SerializeField] private Transform _bubbleEndPosition;
        [SerializeField] private RectTransform _container;
        [SerializeField] private BaseButton _adPlayButton;

        private ScaleGuiAnim _anim;
        private GameModel _gameModel;
        private GameCamera _gameCamera;
        private AdModel _ad;
        private Tween _fillTween;
        private Tween _progressTween;
        private int _rewardAmount;
        private int _gemsCount;
        private Vector2 _bubbleStartPosition;
        
        private string[] _nicknames;
        private int _currentPlace;
        private RatingItem _playerRatingItem;
        private RatingItem[] _ratingItems;
        private float _scrollItemSize;
        private int _startRating;
        private bool _adPlayed;
        private bool _animating;
        
        private const float TWEEN_TIME = 1.5f;
        
        public override void Init()
        {
            _anim = GetComponent<ScaleGuiAnim>();
            _nextLevelButton.SetCallback(CalculateEarned);
            _gameModel = Models.Get<GameModel>();
            _gameCamera = GameCamera.Instance;
            _ad = Models.Get<AdModel>();
            _adPlayButton.SetCallback(AdPlayButtonPressed);
            _bubbleStartPosition = _earnedTextCopy.transform.localPosition;
            _currentPlace = _gameModel.GetParam(GameParamType.RatingPlace).IntValue;
            _startRating = GameBalance.Instance.StartRatingPlace;
            if (_nicknames == null || _nicknames.Length == 0)
            {
                _nicknames = _gameModel.GenerateNickNames(60);
                _playerRatingItem = GetComponentInChildren<RatingItem>();
                _playerRatingItem.Init(_currentPlace, this);
                _playerRatingItem.Activate();
                _ratingItems = new RatingItem[10];
                DrawRatingItems();
                _scrollItemSize = _ratingItems[0].Spacing + _ratingItems[0].GetComponent<RectTransform>().sizeDelta.y;
            }
            base.Init();
        }

        public override void UpdateMe(float deltaTime)
        {
            if (_ratingItems.Length > 0)
            {
                foreach (var item in _ratingItems)
                {
                    item.UpdateMe(deltaTime);
                }
            }
            base.UpdateMe(deltaTime);
        }

        private void DrawRatingItems()
        {
            for (int i = 0; i < _ratingItems.Length; i++)
            {
                if (_ratingItems[i] == null)
                {
                    _ratingItems[i] = Pool.Pop<RatingItem>(_container);
                }
                _ratingItems[i].Init(_currentPlace - 5 + i, this);
                _ratingItems[i].MoveTo(i - 3);
                _ratingItems[i].Activate();
            }
            _playerRatingItem.ResetUI(
                _gameModel.GetParam(GameParamType.RatingPlace).IntValue,
                "You",
                (int)_gameModel.GetProgress(GameParamType.RatingExperience).CurrentValue
            );
        }

        private void OnPressedNextLevel()
        {
            Close();
            _gameModel.LoadNextLevel();
        }
        
        public override BaseUI Open()
        {
            _nextLevelButton.transform.localScale = Vector3.zero;

            var coins = _gameModel.GetParam(GameParamType.CollectedCoins).IntValue;
            _rewardAmount = GameBalance.Instance.RewardSoftCurrencyForLevel + coins * GameBalance.Instance.RewardSoftCurrencyForCoin;

            var soft = 0;
            _gemPriceText.SetText($"{soft} <sprite name=coin>");
            _earnedTextCopy.SetText("");
            _earnedTextCopy.transform.localPosition = _bubbleStartPosition;
            _earnedTextCopy.enabled = false;

            GameSounds.Instance.PlaySound(GameSoundType.GetSoft);
            
            DOTween.To(() => soft, x =>
                {
                    soft = x;
                    _gemPriceText.SetText($"{soft}");
                }, _rewardAmount, TWEEN_TIME)
                .SetEase(Ease.Linear)
                .OnComplete(ShowNextLevelButton);
            
            _soft.SetText($"{_gameModel.GetParam(GameParamType.Soft).Value}");
            
            _container.anchoredPosition = new Vector2(_container.anchoredPosition.x, 0f);
            _gameModel.GetProgress(GameParamType.RatingExperience).Change(_rewardAmount);
            DrawRatingItems();
            AnimateRating();
            _adPlayButton.Deactivate();
            _nextLevelButton.Deactivate();
            _adPlayed = false;
            
            // _gameCamera.ShowFx();
            _anim.PlayAnim(GuiAnimType.Open);
            return base.Open();
        }
        
        private void AnimateRating()
        {
            var seq = DOTween.Sequence();
            seq.Append(_playerRatingItem.transform.DOScale(Vector3.one * 1.2f, 0.5f));
            seq.Append(_playerRatingItem.transform.DOScale(Vector3.one * 1.2f, 1f));
            seq.Append(_playerRatingItem.transform.DOScale(Vector3.one, 0.5f));
            seq.onComplete = StopAnimating;
            var rating = (int)_gameModel.GetProgress(GameParamType.RatingExperience).CurrentValue;
            var currentPlace = rating / 10;
            _gameModel.GetParam(GameParamType.RatingPlace).SetValue(_startRating - currentPlace);
            var newPlace = _gameModel.GetParam(GameParamType.RatingPlace).IntValue;
            for (int i = 0; i < _ratingItems.Length; i++)
            {
                _ratingItems[i].SetInvisible(newPlace);
            }
            Scroll(newPlace  - _currentPlace);
            _currentPlace = newPlace;
            _playerRatingItem.ResetUI(newPlace, "You", rating);
            _animating = true;
        }

        private void StopAnimating()
        {
            _animating = false;
            if (!_adPlayed)
            {
                _adPlayButton.Activate();
            }
            _nextLevelButton.Activate();
        }

        private void Scroll(int delta)
        {
            _container.DOLocalMoveY( _container.localPosition.y + delta * _scrollItemSize, 2f);
        }
        
        private void AdPlayButtonPressed()
        {
            _ad.AdEvent += OnAdFinished;
            _ad.ShowAd(AdPlacement.LevelCompletedReward);
        }
        
        private void OnAdFinished(bool success)
        {
            _ad.AdEvent -= OnAdFinished;
            
            if (_adPlayed) 
                return;
           
            if (success)
            {
                _gameModel.AddSoft(_rewardAmount * 2);
                _gameModel.AddExp(_rewardAmount * 2 / 3);
                _gemPriceText.SetText($"{_rewardAmount * 3}");
                AnimateRating();
                
                GameSounds.Instance.PlaySound(GameSoundType.GetSoftFromAd);
            }

            _adPlayed = true;
            _adPlayButton.Deactivate();
            _nextLevelButton.SetText("Continue");
        }

        private void ShowNextLevelButton()
        {
            _nextLevelButton.transform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.Linear)
                .OnComplete(() => _nextLevelButton.SetInteractable(true));
        }

        private void CalculateEarned()
        {
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
            _nextLevelButton.SetInteractable(false);
            
            _earnedTextCopy.SetText($"+{_rewardAmount} <sprite name=coin>");
           
            var sequence = DOTween.Sequence();
            sequence.Append(
                _earnedTextCopy.transform
                    .DOMove(_bubbleEndPosition.position, 0.3f)
                    .SetEase(Ease.Linear)
                    .OnStart(() => { _earnedTextCopy.enabled = true; })
                    .OnComplete(() =>
                    {
                        _earnedTextCopy.enabled = false;
                        var soft = _gameModel.GetParam(GameParamType.Soft);
                        soft.Change(_rewardAmount);
                        _soft.SetText(soft.Value.ToString());
                        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                    })
            );
            sequence.AppendInterval(0.5f);
            sequence.OnComplete(OnPressedNextLevel);
        }
        
        public string GetNickname(int place)
        {
            if (place > _nicknames.Length)
            {
                place %= _nicknames.Length;
            }
            return _nicknames[place];
        }

        public override void Close()
        {
            // _gameCamera.HideFx();
            base.Close();
        }
    }
}