using _Idle.Scripts.Balance;
using GeneralTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.Windows.DailyRewards
{
    public class RewardItem : BaseUIBehaviour
    {
        [SerializeField] private TextMeshProUGUI _amount;
        [SerializeField] private Image _icon;

        public void Show(RewardItemConfig config)
        {
            _amount.text = $"{config.Amount}";
            _icon.sprite = GameResources.Instance.GetSprite(config.RewardType);
        }
    }
}