using System.Collections.Generic;
using GeneralTools.Localization;
using GeneralTools.Tools;
using GeneralTools.UI;
using Plugins.GeneralTools.Scripts.UI;
using TMPro;
using UnityEngine;

namespace _Idle.Scripts.UI.Windows
{
    public class RateGameWindow : BaseWindow
    {
        [SerializeField] private BaseButton _btnRate;
        [SerializeField] private List<BaseButton> _stars;
        
        private int _starCount;
        
        public override void Init()
        {
            foreach (var star in _stars)
            {
                star.SetCallback(() => OnPressedStar(star));
            }
            
            _btnRate.SetCallback(OnPressedRate);

            base.Init();
        }

        public override BaseUI Open()
        {
            _starCount = 4;
            PlayerPrefs.SetString("IsAppRatedShowed", "");
            RedrawStars();
            return base.Open();
        }

        protected override void OnPressedClose()
        {
            AppEventsProvider.TriggerEvent(GameEvents.Rate_us, 0);
            base.OnPressedClose();
        }

        private void OnPressedStar(BaseButton _btn)
        {
            _starCount = _stars.IndexOf(_btn);
            RedrawStars();
        }

        private void RedrawStars()
        {
            for (int i = 0; i < _stars.Count; i++)
            {
                if (i <= _starCount)
                {
                    _stars[i].SetSpriteColor(Color.white);
                }
                else
                {
                    _stars[i].SetSpriteColor(Color.gray);
                }
            }
        }

        private void OnPressedRate()
        {
            if (_starCount == 4)
            {
                Application.OpenURL("https://play.google.com/store/apps/details?id=com.platypus.games.idle.looters");
            }
            
            AppEventsProvider.TriggerEvent(GameEvents.Rate_us, _starCount + 1);
            PlayerPrefs.SetString("IsAppRated", "");
            Close();
        }
    }
}