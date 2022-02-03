using System;
using System.Collections.Generic;
using System.Linq;

namespace _Idle.Scripts.Analytics
{
    public abstract class AnalyticsWrapper
    {
        public abstract void SendEvent(string eventType, params (string, object)[] args);

        public abstract void Init();

        public virtual void ForceSend()
        {
        }

        public virtual void TutorialStart()
        {
        }

        public virtual void TutorialComplete()
        {
        }

        public virtual void TutorialStep(int step)
        {
        }

        protected Dictionary<string, object> ToDictionary(params (string, object)[] args)
        {
            return args.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        public virtual void CustomEventProcess(Enum eventType, params object[] parameters)
        {
        }
    }
}