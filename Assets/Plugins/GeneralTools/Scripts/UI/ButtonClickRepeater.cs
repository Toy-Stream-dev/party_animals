using GeneralTools.Tools.ExtensionMethods;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GeneralTools.UI
{
	public class ButtonClickRepeater : BaseUIBehaviour
	{
		private enum State
		{
			WaitingForInitialization,
			Waiting,
			WaitingFirstDelay,
			WaitingRepeatDelay,
			Disabled
		}

		[SerializeField] private float _firstDelay = 0.8f;
		[SerializeField] private float _repeatDelay = 0.2f;

		private BaseButton _baseButton;
		private State _state;
		private float _leftDelay;

		public bool Break { get; private set; }
		public float LeftDelay => _leftDelay;

		private void OnEnable()
		{
			if (_state != State.WaitingForInitialization)
			{
				_state = State.Waiting;
				return;
			}

			_baseButton = GetComponent<BaseButton>();

			if (_baseButton == null)
			{
				_state = State.Disabled;
				return;
			}

			_baseButton.InteractableChangedEvent += () =>
			{
				if (_baseButton.Interactable)
				{
					_leftDelay = _repeatDelay;
				}
				else
				{
					OnEnd();
				}
			};

			_state = State.Waiting;

			var eventTrigger = GetComponent<EventTrigger>();
			if (eventTrigger == null) eventTrigger = gameObject.AddComponent<EventTrigger>();

			eventTrigger.AddCallback(EventTriggerType.PointerDown, OnStart);
			eventTrigger.AddCallback(EventTriggerType.PointerUp, OnEnd);
			eventTrigger.AddCallback(EventTriggerType.PointerExit, OnEnd);
		}

		private void Update()
		{
			if (Break) return;
			if (_state != State.WaitingFirstDelay && _state != State.WaitingRepeatDelay) return;

			if (_leftDelay < 0) return;
			_leftDelay -= Time.deltaTime;
			if (_leftDelay > 0) return;
			
			_baseButton.SimulateClick();
			_baseButton.SkipUnityButtonClickCallback = true;
		}

		public void Reset()
		{
			Break = true;
		}

		private void OnStart()
		{
			Break = false;
			_state = State.WaitingFirstDelay;
			_leftDelay = _firstDelay;
		}

		private void OnEnd()
		{
			_state = State.Waiting;
			//_leftDelay = 0;
		}
	}
}