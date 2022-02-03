using System;
using System.Collections.Generic;
using GeneralTools.Tools;
using GeneralTools.UI;
using GeneralTools.UI.Animations;
using UnityEngine;

namespace Plugins.GeneralTools.Scripts.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class BaseWindow : BaseUI
	{
		private const string INNER_WINDOW = "Window";

		public static event Action InterruptInputEvent;
		public static event Action<BaseUI> WindowOpenedEvent, WindowClosedEvent;

		private static readonly List<BaseWindow> _allOpened = new List<BaseWindow>(),
		                                         _windowsStack = new List<BaseWindow>();
		private static BaseWindow _topWindow;

		public event Action<BaseWindow> AboveWindowOpened, AboveWindowClosed;

		protected CloseWindowButton CloseButton { get; private set; }
		public CanvasGroup CanvasGroup { get; protected set; }
		protected WindowBack WindowBack  { get; private set; }

		public static bool ForceWindowAnims { get; set; }
		protected virtual bool DontSkipAnims => false;
		
		[SerializeField] private bool _addToOpenedStack = true;

		public bool IsTop => _topWindow == this;
		public BaseWindow TopWindow => _topWindow;

		public override void Init()
		{
			if (!IsNotInitialized)
			{
				Debug.Log($"Calling Init() for already initialized ui ({GetType().Name})", gameObject);
				return;
			}

			CloseButton = GetComponentInChildren<CloseWindowButton>(true);
			WindowBack = GetComponentInChildren<WindowBack>(true);
			CanvasGroup = GetComponent<CanvasGroup>();

			if (CloseButton != null) CloseButton.Callback = OnPressedClose;
			if (WindowBack != null) WindowBack.Init(CloseButton);

			base.Init();

			CheckCustomGuiAnims();
		}

		private void CheckCustomGuiAnims()
		{
			if (CustomAnims.Count > 0) return;

			RectTransform innerWindow = null;

			foreach (Transform child in transform)
			{
				if (child.name != INNER_WINDOW) continue;
				innerWindow = child as RectTransform;
				break;
			}

			if (innerWindow == null) return;

			var sourceAnim = GameUI.GetCustomGuiAnimSetup<ScaleGuiAnim>();
			var anim = gameObject.AddComponent<ScaleGuiAnim>()
			                     .CopyFrom(sourceAnim);

			anim.SetTarget(innerWindow);

			CustomAnims.Add(anim);
		}

		public override BaseUI Open()
		{
			if (IsNotInitialized)
			{
				Debug.LogWarning($"Trying to open not initialized {this}");
				Init();
			}

			if (IsOpened)
			{
				Debug.LogWarning($"Gui {GetType().Name} is already opened");
			}

			AddToStackIfNeeded();

			if (!_allOpened.Contains(this)) _allOpened.Add(this);

			if (WindowBack != null) WindowBack.Activate();
			SetBlockRaycasts(true);
			if (CanvasGroup != null) CanvasGroup.blocksRaycasts = true;
			return base.Open();
		}

		private void AddToStackIfNeeded()
		{
			if (!_addToOpenedStack || _windowsStack.Contains(this)) return;
			var prevWindow = _windowsStack.LastValue();
			if (prevWindow != null)
			{
				prevWindow.OnAboveWindowOpened(this);
			}

			_windowsStack.Add(this);
			_topWindow = this;
		}

		protected override void OnAnimStarted()
		{
			base.OnAnimStarted();
			SetBlockRaycasts(false);
		}

		protected override void OnOpened()
		{
			base.OnOpened();
			SetBlockRaycasts(true);
			WindowOpenedEvent?.Invoke(this);
		}

		public override void Close()
		{
			if (State == GUIState.Closed) return;

			if (_allOpened.Contains(this)) _allOpened.Remove(this);

			InterruptInputEvent?.Invoke();
			if (WindowBack != null) WindowBack.Deactivate();
			base.Close();
			WindowClosedEvent?.Invoke(this);

			RemoveFromStackIfNeeded();
		}

		private void RemoveFromStackIfNeeded()
		{
			if (!_addToOpenedStack || !_windowsStack.Contains(this)) return;
			var prevIndex = _windowsStack.IndexOf(this) - 1;
			_windowsStack.Remove(this);
			var prevWindow = prevIndex < 0 ? null : _windowsStack[prevIndex];
			_topWindow = _windowsStack.LastValue();
			if (prevWindow == _topWindow && _topWindow != null)
			{
				_topWindow.OnAboveWindowClosed(this);
			}
		}

		public override void UpdateMe(float deltaTime)
		{
			if (!IsTop) return;
			
			foreach (var button in AllButtons)
			{
				button.UpdateMe(deltaTime);
			}
		}

		protected virtual void OnAboveWindowOpened(BaseWindow window)
		{
			SetBlockRaycasts(false);
			AboveWindowOpened?.Invoke(window);
		}

		protected virtual  void OnAboveWindowClosed(BaseWindow window)
		{
			SetBlockRaycasts(true);
			AboveWindowClosed?.Invoke(window);
		}

		private void SetBlockRaycasts(bool block)
		{
			if (CanvasGroup != null) CanvasGroup.blocksRaycasts = block;
		}

		protected virtual void OnPressedClose()
		{
			Close();
		}
		
		public static void CloseAll()
		{
			while (_allOpened.Count > 0) _allOpened.LastValue().Close();
		}
		
		public void CloseAllInStack()
		{
			while (_windowsStack.Count > 0) _windowsStack.LastValue().Close();
		}

		public static void CloseWindowBefore(BaseWindow except = null)
		{
			for (int i = _windowsStack.Count - 1; i >= 0; i--)
			{
				if (_windowsStack[i] == except) continue;
				_windowsStack[i].Close();
				break;
			}
		}

		protected override void PlayAnim(GuiAnimType animType, Action callback = null, float delay = 0)
		{
			if (ForceWindowAnims && !DontSkipAnims)
			{
				ForceAnim(animType, callback);
				return;
			}
			base.PlayAnim(animType, callback, delay);
		}

		public List<BaseWindow> GetOpenedWindows()
		{
			return _windowsStack;
		}
	}
}