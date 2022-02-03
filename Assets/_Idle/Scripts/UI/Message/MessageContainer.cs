using System.Collections.Generic;
using GeneralTools.Pooling;
using GeneralTools.Tools;
using Plugins.GeneralTools.Scripts.UI;

namespace _Idle.Scripts.UI.Message
{
    public class MessageContainer : BaseWindow
    {
        private readonly List<MessageUI> _messages = new List<MessageUI>();

        public override void Init()
        {
            Pool.Spawn<MessageUI>(30);
            base.Init();
            
            this.Activate();
        }

        public void Show(string text)
        {
            var i = _messages.Count;
            foreach (var message in _messages)
            {
                message.Move(120 * i);
                i--;
            }

            var newMessage = Pool.Pop<MessageUI>(transform);
            newMessage.Play(text, newMessage.rectTransform);
            newMessage.OnEventCompleted += OnShowed;

            _messages.Add(newMessage);
        }

        private void OnShowed(MessageUI bubble)
        {
            bubble.OnEventCompleted -= OnShowed;
            bubble.PushToPool();
            
            _messages.Remove(bubble);
        }
    }
}