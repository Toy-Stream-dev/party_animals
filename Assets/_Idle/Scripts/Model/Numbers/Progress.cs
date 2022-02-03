using System;
using UnityEngine;

namespace _Idle.Scripts.Model.Numbers
{
	[Serializable]
	public class Progress
	{
		public enum State
		{
			None,
			Active,
			Completed,
			Paused
		}

		public event Action UpdatedEvent,
		                    CompletedEvent;

		public event Action<State> StateChangedEvent;

		[SerializeField] private BigNumber _target, _current, _deltaK;
		[SerializeField] private bool _looped;
		[SerializeField] private State _state;

		public BigNumber DeltaK => _deltaK;
		public BigNumber Target => _target;

		public bool IsCompleted => _state == State.Completed;
		public bool IsPaused => _state == State.Paused;

		public double ProgressValue => _current.Value != 0 ? _current.Value / _target.Value : 0;
		public double CurrentValue => _current.Value;
		public double TargetValue => _target.Value;
		public bool Looped => _looped;

		public Progress(BigNumber current, BigNumber target)
		{
			_state = State.Active;

			_current = current;
			_target = target;

			_current.UpdatedEvent += () => UpdatedEvent?.Invoke();
			_target.UpdatedEvent += () => UpdatedEvent?.Invoke();
		}

		public Progress(BigNumber target, bool looped)
		{
			_state = State.Active;

			_target = target;
			_looped = looped;

			_deltaK = 1;
			_current = 0;

			_target.UpdatedEvent += CheckProgress;
		}
		
		public Progress(BigNumber current, BigNumber target, bool looped)
		{
			_state = State.Active;
		
			_current = current;
			_target = target;
			_looped = looped;
		
			_deltaK = 1;
			//_current = 0;
		
			_target.UpdatedEvent += CheckProgress;
		}

		public void UpdateEvent()
		{
			_target.UpdatedEvent += CheckProgress;
		}

		public void SetTargetValue(double target)
		{
			_target = target;
			CheckProgress();
		}

		public void SetValue(double value, bool update = true)
		{
			_current.SetValue(value);
			if (update) CheckProgress();
		}

		public void Change(BigNumber delta)
		{
			Change(delta.Value);
		}

		public void Change(double delta, bool checkProgress = true)
		{
			if (_state != State.Active)
			{
				return;
			}
			
			_current.Change(delta * _deltaK.Value);
			
			UpdatedEvent?.Invoke();

			if (checkProgress) CheckProgress();
		}

		private void CheckProgress()
		{
			if (_state != State.Active) return;

			if (_current.Value < _target.Value)
			{
				UpdatedEvent?.Invoke();
				return;
			}

			if (_looped)
			{
				_current.SetValue(0);
			}
			else
			{
				_current.SetValue(_target.Value);
				SetState(State.Completed);
			}
			
			UpdatedEvent?.Invoke();
			CompletedEvent?.Invoke();
		}

		public Progress Pause()
		{
			if (_state == State.Active)
			{
				SetState(State.Paused);
			}

			return this;
		}

		public Progress Play()
		{
			if (_state == State.Paused || _state == State.Completed)
			{
				SetState(State.Active);
			}

			return this;
		}

		public Progress Reset()
		{
			_state = State.Active;
			_current.SetValue(0);
			CheckProgress();
			return this;
		}

		private void SetState(State state)
		{
			if (_state == state) return;

			_state = state;

			StateChangedEvent?.Invoke(_state);
		}

		public void CopyFrom(GameProgress source)
		{
			_target.CopyFrom(source._target);
			_current.CopyFrom(source._current);
			_deltaK.CopyFrom(source._deltaK);

			_looped = source._looped;

			SetState(source._state);
		}

		public override string ToString()
		{
			return $"{_current} / {_target}";
		}
	}
}