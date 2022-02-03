using _Idle.Scripts.Model;
using GeneralTools.Tools;
using GeneralTools.UI;
using TMPro;
using UnityEngine;

namespace _Idle.Scripts.UI
{
    public enum BadgeNotificationType
    {
        None,
    }
    
    public class BadgeNotification : BaseUIBehaviour
    {
        public BadgeNotificationType Type => _type;
        
        [SerializeField] private BadgeNotificationType _type;
        
        private TextMeshProUGUI _text;
        
        public void Redraw(GameModel gameModel)
        {
            var count = gameModel.GetBadgeCount(_type);
            Redraw(count);
        }

        public void Redraw(int count)
        {
            if (count == 0)
            {
                Hide();
                return;
            }

            this.Activate();

            if (_text == null) _text = GetComponentInChildren<TextMeshProUGUI>();
            _text.text = count.ToString();
        }

        private void Hide()
        {
            this.Deactivate();
        }
    }
}