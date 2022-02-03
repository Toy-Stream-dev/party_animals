using System;
using GeneralTools.Model;
using UnityEngine;

namespace _Idle.Scripts.Model.Components.Timer
{
	public class TimerComponent : BaseComponent
	{
		private readonly float _timeInterval;
		private float _timeleft;
		private Action _callback;
		private readonly bool _callOnce;
		
		public TimerComponent(BaseModel model, float timeOut, Action callback) : base(model)
		{
			_timeInterval = timeOut;
			_timeleft = timeOut;
			_callback = callback;
		}

		public TimerComponent(BaseModel model, float timeOut, Action callback, bool callOnce = false) : this(model, timeOut, callback)
		{
			_callOnce = callOnce;
		}

		public override void Update(float deltaTime)
		{
			_timeleft -= deltaTime;
			
			if (_timeleft > 0.0f)
				return;

			_timeleft = _timeInterval;
			_callback?.Invoke();

			if (!_callOnce) 
				return;
			
			Stop();
		}

		public void Stop()
		{
			Model.RemoveComponent<TimerComponent>();
			_callback = null;
		}
	}
}