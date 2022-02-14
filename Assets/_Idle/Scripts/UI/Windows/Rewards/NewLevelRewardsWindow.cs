using System.Collections.Generic;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.Saves;
using _Idle.Scripts.UI.ResourceBubble;
using GeneralTools.Model;
using GeneralTools.Pooling;
using GeneralTools.UI;
using TMPro;
using UnityEngine;

namespace _Idle.Scripts.UI.Windows.DailyRewards
{
    public class NewLevelRewardsWindow : BaseSoundableWindow
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private BaseButton _getButton;
        [SerializeField] private Transform _container;
        
        private readonly List<RewardItem> _items = new List<RewardItem>();

        private GameModel _gameModel;
        private GameBalance _balance;
        private CharacterLevel _characterLevel;

        public override void Init()
        {
            _getButton.SetCallback(OnPressedGet);
            
            _gameModel = Models.Get<GameModel>();
            _balance = GameBalance.Instance;

            base.Init();
        }

        public void Open(CharacterLevel characterLevel)
        {
            _characterLevel = characterLevel;
            
            var rewards = _characterLevel.Rewards;
            for (var i = 0; i < rewards.Length; i++)
            {
                var item = Pool.Pop<RewardItem>(_container);
                item.Show(rewards[i]);
                _items.Add(item);
            }
            
            base.Open();
        }

        private void OnPressedGet()
        {
            foreach (var reward in _characterLevel.Rewards)
            {
                switch (reward.RewardType)
                {
                    case GameParamType.Soft:
                        _gameModel.AddSoft(reward.Amount);
                        GameUI.Get<ResourceBubblesContainer>().Show(BubbleTypes.Soft, reward.Amount, _getButton.rectTransform.position);
                        break;
                    case GameParamType.Hard:
                        _gameModel.AddHard(reward.Amount);
                        GameUI.Get<ResourceBubblesContainer>().Show(BubbleTypes.Hard, reward.Amount, _getButton.rectTransform.position);
                        break;
                }
            }
            
            GameSounds.Instance.PlaySound(GameSoundType.GetSoftFromAd);

            GameSave.Save();
            
            Close();
        }
        
        public override void Close()
        {
            foreach (var rewardItem in _items)
            {
                rewardItem.PushToPool();
            }
            _items.Clear();
            
            base.Close();
        }
    }
}