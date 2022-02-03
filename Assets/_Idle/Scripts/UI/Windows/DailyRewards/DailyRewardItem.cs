using _Idle.Scripts.Balance;
using GeneralTools.Localization;
using GeneralTools.Tools;
using GeneralTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.Windows.DailyRewards
{
    public class DailyRewardItem : BaseUIBehaviour
    {
        [SerializeField] private TextMeshProUGUI _day, _amount1, _amount2;
        [SerializeField] private Image _mark, _titleImage;
        [SerializeField] private Color _currentItemColor;
        [SerializeField] private Color _nextItemColor;

        public void Show(int day, bool received, bool nextDay, DailyRewardConfig config)
        {
            _titleImage.color = nextDay || received ? _nextItemColor : _currentItemColor;
            _day.text = $"{"day".Localized()} {day}";
            _amount1.text = $"{config.Amount}";
            _mark.SetActive(received);
        }
    }
}