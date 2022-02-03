using System;
using System.Collections.Generic;
using _Idle.Scripts.Ad;
using GeneralTools.Model;
using GeneralTools.Tools;

namespace _Idle.Scripts.Analytics
{
    public struct InAppPurchaseData
    {
        public string Currency,
            Revenue,
            Quantity,
            ID,
            Transaction;
    }
    
    public class AnalyticsModel : BaseModel
    {
        private List<AnalyticsWrapper> _analyticsWrappers;

        public override BaseModel Init()
        {
// #if UNITY_EDITOR
//             return this;
// #endif
            AppEventsProvider.Action += OnAction;

            _analyticsWrappers = new List<AnalyticsWrapper>()
            {
                //new AppMetricAnalyticsWrapper(),
                //new AppsFlyerAnalyticsWrapper()
                new GameAnalyticsWrapper(),
                new TenjinAnalitycsWrapper(),
                new FirebaseAnalyticsWrapper()
            };

            foreach (var analyticsWrapper in _analyticsWrappers) analyticsWrapper.Init();

            return this;
        }
        
        private void OnAction(Enum eventType, object[] parameters)
        {
            foreach (var wrapper in _analyticsWrappers)
            {
                wrapper.CustomEventProcess(eventType, parameters);
            }
        }
        
        private string GetEventName(Enum eventType) => $"{eventType}";

        private void SendEvent(string eventName, params (string, object)[] parameters)
        {
            foreach (var analytic in _analyticsWrappers)
            {
                analytic.SendEvent(eventName, parameters);
            }
        }
    }
}