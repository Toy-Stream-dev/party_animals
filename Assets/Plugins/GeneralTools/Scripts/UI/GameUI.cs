using System;
using System.Collections.Generic;
using System.Linq;
using GeneralTools.Tools;
using GeneralTools.UI.Animations;
using Plugins.GeneralTools.Scripts.UI;
using UnityEngine;

namespace GeneralTools.UI
{
	public class GameUI : BaseBehaviour
	{
		public static Transform Root { get; private set; }
		public static bool IsInitialized { get; private set; }
		
		private static readonly List<BaseUI> _allGuis = new List<BaseUI>();
		private static readonly List<BaseWindow> _allWindows = new List<BaseWindow>();
		private static readonly Dictionary<Type, BaseUI> _cachedGuis = new Dictionary<Type, BaseUI>();
		private static readonly List<BaseUI> _updatableGuis = new List<BaseUI>();

		private static readonly List<CustomGuiAnim> _customGuiAnims = new List<CustomGuiAnim>();

		private void Awake()
		{
			Root = transform;
			InspectChildren();
		}

		private void InspectChildren()
		{
			_allWindows.AddRange(GetComponentsInChildren<BaseWindow>(true));
			_allGuis.AddRange(GetComponentsInChildren<BaseUI>(true));
			_customGuiAnims.AddRange(GetComponents<CustomGuiAnim>());

			foreach (var window in _allWindows) window.Deactivate();
		}

		public static void UpdateLocalization()
		{
			foreach (var gui in _allGuis)
			{
				gui.UpdateLocalization();
			}
		}

		public static T GetCustomGuiAnimSetup<T>() where T : CustomGuiAnim
		{
			return _customGuiAnims.Find(a => a is T) as T;
		}

		public static void Init()
		{
			if (IsInitialized) return;

			foreach (var gui in _allWindows)
			{
				if (gui.IsNotInitialized) gui.Init();
			}

			IsInitialized = true;
		}

		public static T Get<T>() where T : BaseUI
		{
			var type = typeof(T);

			if (_cachedGuis.ContainsKey(type)) return _cachedGuis[type] as T;

			foreach (var gui in _allGuis)
			{
				if (gui is T targetGui)
				{
					_cachedGuis.Add(type, targetGui);
					return targetGui;
				}
			}

			var newGui = Prefabs.CopyPrefab<T>(Root);
			if (newGui == null) return null;

			newGui.Init();

			_allGuis.Add(newGui);
			_cachedGuis.Add(type, newGui);
			return newGui;
		}

		public static T GetBeforeInit<T>() where T : BaseWindow
		{
			foreach (var gui in _allWindows)
			{
				if (gui is T targetGui)
				{
					return targetGui;
				}
			}

			var newGui = Prefabs.CopyPrefab<T>(Root);
			if (newGui == null) return null;

			_allWindows.Add(newGui);
			return newGui;
		}

		public static T Load<T>(Transform parent) where T : BaseUI
		{
			var type = typeof(T);
			var newGui = Prefabs.CopyPrefab<T>(parent);
			if (newGui == null) return null;

			newGui.Init();

			_allGuis.Add(newGui);
			_cachedGuis.Add(type, newGui);
			return newGui;
		}

		public static List<T> GetAll<T>() where T : BaseUI
		{
			return _allGuis.OfType<T>().ToList();
		}

		public new static void UpdateMe(float deltaTime)
		{
			for (var i = 0; i < _updatableGuis.Count; i++)
			{
				_updatableGuis[i].UpdateMe(deltaTime);
			}
		}

		public new static void LateUpdateMe()
		{
			for (var i = 0; i < _updatableGuis.Count; i++)
			{
				_updatableGuis[i].LateUpdateMe();
			}
		}

		public static void RegisterUpdatableGUI(BaseUI ui)
		{
			if (!_updatableGuis.Contains(ui)) _updatableGuis.Add(ui);
		}

		public static void UnregisterUpdatableGUI(BaseUI ui)
		{
			if (_updatableGuis.Contains(ui)) _updatableGuis.Remove(ui);
		}

		public static void Destroy<T>() where T : BaseUI
		{
			T target = null;
			foreach (var baseGui in _allGuis)
			{
				if (!(baseGui is T gui)) continue;
				target = gui;
				break;
			}

			if (target == null) return;
			if (target.IsOpened) target.Close();

			if (_allGuis.Contains(target)) _allGuis.Remove(target);
			if (target is BaseWindow window && _allWindows.Contains(window)) _allWindows.Remove(window);

			var type = typeof(T);
			if (_cachedGuis.ContainsKey(type)) _cachedGuis.Remove(type);

			target.DestroyGO();
		}

		public void CloseAllWindows()
		{
			foreach (var window in _allWindows)
			{
				if (window.IsOpened) window.Close();
			}
		}
	}
}