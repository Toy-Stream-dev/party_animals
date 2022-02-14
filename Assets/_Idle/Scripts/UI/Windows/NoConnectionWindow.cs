using System;
using GeneralTools.UI;
using Plugins.GeneralTools.Scripts.UI;
using TMPro;
using UnityEngine;

namespace _Idle.Scripts.UI.Windows
{
    public class NoConnectionWindow : BaseWindow
    {
        public Action OnConnected;
        
        [SerializeField] private TextMeshProUGUI _text1;
        [SerializeField] private TextMeshProUGUI _text2;
        [SerializeField] private BaseButton _checkButton;

        public override void Init()
        {
            _checkButton.SetCallback(OnPressedCheck);
            base.Init();
        }

        private void OnPressedCheck()
        {
            MainGame.CheckConnection();
        }

        private void OnCheckedConnection(bool isConnected)
        {
            if (!isConnected) return;
            OnConnected?.Invoke();
            Close();
        }

        public override BaseUI Open()
        {
            MainGame.OnConnected += OnCheckedConnection;
            return base.Open();
        }

        public override void Close()
        {
            MainGame.OnConnected -= OnCheckedConnection;
            base.Close();
        }
    }
}