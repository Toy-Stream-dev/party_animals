using _Idle.Scripts.Ad;
using _Idle.Scripts.Model;
using DG.Tweening;
using GeneralTools.Model;
using GeneralTools.UI;
using Plugins.GeneralTools.Scripts.UI;
using UnityEngine;

namespace _Idle.Scripts.UI.Windows
{
    public class LevelFailedWindow : BaseWindow
    {
        [SerializeField] private BaseButton _adPlayButton;
        [SerializeField] private BaseButton _restartLevelButton;
        
        private GameModel _gameModel;
        private AdModel _ad;
        
        private const float TWEEN_TIME = 0.4f;
        
        public override void Init()
        {
            _adPlayButton.SetCallback(AdPlayButtonPressed);
            _restartLevelButton.SetCallback(OnPressedRestartLevel);
            _gameModel = Models.Get<GameModel>();
            _ad = Models.Get<AdModel>();
            
            base.Init();
        }

        private void OnPressedRestartLevel()
        {
            _ad.ShowAd(AdPlacement.LevelFailed, AdVideoType.Interstitial);
            
            _gameModel.RestartLevel();
            Close();
        }
        
        public override BaseUI Open()
        {
            ShowRestartButton();
                
            return base.Open();
        }

        private void ShowRestartButton()
        {
            _restartLevelButton.transform.localScale = Vector3.zero;
            _restartLevelButton.SetInteractable(false);
            
            _restartLevelButton.transform.DOScale(Vector3.one, TWEEN_TIME)
                .SetDelay(1.0f)
                .SetEase(Ease.Linear)
                .OnComplete(() => _restartLevelButton.SetInteractable(true));
        }
        
        private void AdPlayButtonPressed()
        {
            _ad.AdEvent += OnAdFinished;
            _ad.ShowAd(AdPlacement.Revive);
        }
        
        private void OnAdFinished(bool success)
        {
            _ad.AdEvent -= OnAdFinished;
            if (success)
            {
                _gameModel.RevivePlayer();
                Close();
            }
            else
            {
                _gameModel.RestartLevel();
                Close();
            }
        }
    }
}