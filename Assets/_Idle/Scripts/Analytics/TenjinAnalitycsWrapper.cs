using System;
using UnityEngine;

namespace _Idle.Scripts.Analytics
{
    public class TenjinAnalitycsWrapper : AnalyticsWrapper
    {
        private const string KEY = "C1VYE96SJFZIOAGY7NTS96SFAQGXWEHY";

        private BaseTenjin _instance;
        
        public override void Init()
        {
            TenjinConnect();
        }

        private void TenjinConnect() 
        {
            _instance = Tenjin.getInstance(KEY);
            _instance.Connect();
            Debug.LogWarning("Tenjin connect");
        }
       
        public override void SendEvent(string eventType, params (string, object)[] args)
        {
        }

        public override void CustomEventProcess(Enum eventType, params object[] parameters)
        {
            if (!(eventType is GameEvents metaEvents)) return;

            switch (metaEvents)
            {
                case GameEvents.LevelStart:
                    _instance.SendEvent(AnalyticsEvents.LEVEL_START);
                    break;
            }
        }
        
    }
}