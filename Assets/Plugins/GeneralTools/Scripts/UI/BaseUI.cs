using System;
using System.Collections.Generic;
using GeneralTools.UI.Animations;
using UnityEngine;

namespace GeneralTools.UI
{
	public enum GUIState
	{
		NotInitialized,
		Opened,
		Closed
	}

	public class BaseUI : BaseUIBehaviour
	{
		public event Action Initialized = delegate { },
		                    Opened = delegate { },
		                    Closed = delegate { };

		[SerializeField] private bool _updatable;

		public GUIState State { get; private set; }
		
		protected List<BaseButton> AllButtons { get; private set; } = new List<BaseButton>();
		protected int FrameWhenOpened { get; private set; }

		public bool IsNotInitialized => State == GUIState.NotInitialized;
		public bool IsOpened => State == GUIState.Opened;
		public bool IsClosed => State == GUIState.Closed;

		public virtual void Init()
		{
			if (State != GUIState.NotInitialized) return;
			
			State = GUIState.NotInitialized;

			AllButtons = new List<BaseButton>(GetComponentsInChildren<BaseButton>(true));
			
			foreach (var button in AllButtons) button.Init();

			State = GUIState.Closed;
			CacheInfo();
			gameObject.SetActive(false);

			UpdateLocalization();

			Initialized?.Invoke();
		}
		
		public virtual BaseUI Open()
		{
			if (State == GUIState.Opened)
			{
				Debug.LogWarning($"Gui {GetType().Name} is already opened");
			}
			else
			{
				if (_updatable) GameUI.RegisterUpdatableGUI(this);
			}
			
			//Debug.Log($"Gui {GetType().Name} is opened", DebugType.Info);

			FrameWhenOpened = Time.frameCount;
			State = GUIState.Opened;

			PlayAnim(GuiAnimType.Open);

			return this;
		}

		protected override void OnOpened()
		{
			base.OnOpened();
			Opened?.Invoke();
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			Closed?.Invoke();
		}

		public virtual void UpdateLocalization()
		{
			foreach (var button in AllButtons) button.UpdateLocalization();
		}
		
		public override void UpdateMe(float deltaTime)
		{
		
		}

		// public virtual void CloseAnimated(Action onComplete = null)
		// {
		// 	Close();
		// 	onComplete?.Invoke();
		// }

		public virtual void Close()
		{
			if (State == GUIState.Closed) return;
			Reset();
			
			State = GUIState.Closed;
			PlayAnim(GuiAnimType.Close);
			//this.Deactivate();
			
			if (_updatable) GameUI.UnregisterUpdatableGUI(this);
		}

		protected virtual void Reset()
		{
			
		}
	}
}

