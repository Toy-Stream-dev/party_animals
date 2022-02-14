using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.UI.Message;
using GeneralTools.Model;
using GeneralTools.UI;
using TMPro;
using UnityEngine;

namespace _Idle.Scripts.UI.Windows
{
    public class NicknameEnterWindow : BaseSoundableWindow
    {
        [SerializeField] private BaseButton _applyButton;
        private int _softPrice = 500;
        private TMP_InputField _inputField;
        private GameModel _game;

        public override void Init()
        {
            _game = Models.Get<GameModel>();
            _inputField = GetComponentInChildren<TMP_InputField>();
            _applyButton.SetCallback(ApplyButtonPressed);
            base.Init();
        }

        public override BaseUI Open()
        {
            _inputField.text = _game.PlayerNickname;
            return base.Open();
        }

        private void ApplyButtonPressed()
        {
            if (_inputField.text == _game.PlayerNickname)
            {
                Close();
                return;
            }
            
            
            if (_inputField.text.Length < 3)
            {
                GameUI.Get<MessageContainer>().Show("Nickname is too short.");
                return;
            }
            
            if (_softPrice <= _game.GetParam(GameParamType.Soft).IntValue)
            {
                _game.SetPlayerNickname(_inputField.text, _softPrice);
            }
            else
            {
                GameUI.Get<MessageContainer>().Show("Not enough money.");
            }
        }
    }
}
