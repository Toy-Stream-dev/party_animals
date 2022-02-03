using System.Collections.Generic;
using _Idle.Scripts.UI.Hud;
using GeneralTools.Pooling;
using GeneralTools.Tools;
using Plugins.GeneralTools.Scripts.UI;
using UnityEngine;

namespace _Idle.Scripts.UI.Message
{
    public class PopupTextContainer : BaseWindow
    {
        private readonly List<PopupText> _messages = new List<PopupText>();

        public override void Init()
        {
            Pool.Spawn<PopupText>(30);
            base.Init();
            
            this.Activate();
        }

        public void Show(string text)
        {
            var newMessage = Pool.Pop<PopupText>(transform);
            newMessage.SetText(text);
            newMessage.Show();

            _messages.Add(newMessage);
        }
    }
}