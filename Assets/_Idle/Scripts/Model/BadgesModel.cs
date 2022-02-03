using System;
using System.Collections.Generic;
using System.Linq;
using _Idle.Scripts.UI;
using GeneralTools.Model;

namespace _Idle.Scripts.Model
{
    public class BadgesModel : BaseModel
    {
        private Dictionary<BadgeNotificationType, int> _counts;
        private readonly List<BadgeNotificationType> _allTypes = new List<BadgeNotificationType>();

        public override BaseModel Start()
        {
            var elements = Enum.GetValues(typeof(BadgeNotificationType)).Cast<BadgeNotificationType>().ToList();
            foreach (var element in elements)
            {
                _allTypes.Add(element);
            }
            
            _counts = new Dictionary<BadgeNotificationType, int>();

            base.Start();
            return this;
        }

        public void Update(BadgeNotificationType type = BadgeNotificationType.None)
        {
            UpdateBadgeCounts(type);
        }
        
        private void UpdateBadgeCounts(BadgeNotificationType type = BadgeNotificationType.None)
        {
            var count = 0;
            
            if (type == BadgeNotificationType.None)
            {
                foreach (var notificationType in _allTypes)
                {
                    count = CalculateBadgeCount(notificationType);
                    SetCount(notificationType, count);
                }
                return;
            }

            count = CalculateBadgeCount(type);
            SetCount(type, count);
        }
        
        private int CalculateBadgeCount(BadgeNotificationType type)
        {
            switch (type)
            {
            }

            return 0;
        }
        
        private void SetCount(BadgeNotificationType type, int count)
        {
            if (_counts == null) return;
            if (_counts.ContainsKey(type))
            {
                _counts[type] = count;
            }
            else
            {
                _counts.Add(type, count);
            }
        }
        
        public int GetCount(BadgeNotificationType type)
        {
            return _counts.ContainsKey(type) ? _counts[type] : 0;
        }
    }
}