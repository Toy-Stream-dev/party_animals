using System;

namespace GeneralTools.Tools
{
    public static class AppEventsProvider
    {
        public delegate void ActionCallback(Enum type, object[] parameters);
        public static (Enum EventType, object[] Parameters) LastEvent { get; private set; }
        public static event ActionCallback Action;
        
        static AppEventsProvider()
        {
            //Action += OnAction;
        }

        public static void TriggerEvent(Enum action, params object[] list)
        {
            //Debug.Log($"{action} triggered");

            LastEvent = (action, list);
            Action?.Invoke(action, list);
        }
        
        private static void OnAction(Enum action, params object[] list)
        {
            //Debug.Log($"[GameEvent]\nAction: {action} \nParams: {LogTools.ParseIfEnumerable(list)}");
        }
    }
}