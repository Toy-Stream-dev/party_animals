using System;
using System.Collections.Generic;
using System.Linq;
using GeneralTools.Tools;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GeneralTools
{
	[CreateAssetMenu(fileName = "Prefabs", menuName = "General tools/Prefabs", order = 1)]
	public class Prefabs : ScriptableObject
	{
		private static Prefabs _instance;
		private static bool _isInitialized;

		[SerializeField] private string[] _prefabsPaths;
		[SerializeField] private List<BaseBehaviour> _prefabs;

		private static Dictionary<Type, BaseBehaviour> _cachedPrefabsSingle;
		private static Dictionary<Type, List<BaseBehaviour>> _cachedPrefabsGroups;

#if UNITY_EDITOR
		public List<BaseBehaviour> UncachedPrefabs => _prefabs;
#endif

		public bool IsValid(ref List<string> errors)
		{
			var result = true;
			if (errors == null) errors = new List<string>();
			foreach (var prefab in _prefabs.Where(prefab => prefab == null))
			{
				errors.Add($"Missing prefab!");
				result = false;
			}

			return result;
		}

		public static Prefabs Instance
		{
			get
			{
				if (!_isInitialized) Init();
				return _instance;
			}
		}

		public static void Init()
		{
			if (_isInitialized) return;

			_instance = Resources.Load<Prefabs>("Prefabs");

			if (_instance == null) return;

			_isInitialized = true;

#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				_instance.ParsePrefabs();
			}
			else
			{
				_instance.ParsePrefabs();
				_instance.CachePrefabs();
			}
#else
	        _instance.CachePrefabs();
#endif
		}

#if UNITY_EDITOR
		public static void InitOrJustParsePrefabs()
		{
			if (_isInitialized && _instance != null)
			{
				_instance.ParsePrefabs();
			}
			else
			{
				Init();
			}
		}
#endif

		private void CachePrefabs()
		{
			_cachedPrefabsSingle = new Dictionary<Type, BaseBehaviour>();
			_cachedPrefabsGroups = new Dictionary<Type, List<BaseBehaviour>>();

			if (_prefabs.Any(p => p == null))
			{
				Debug.LogError("Prefabs asset has null entries");
				_prefabs.RemoveAll(p => p == null);
			}

			foreach (var prefab in _prefabs)
			{
				var type = prefab.GetType();
				if (_cachedPrefabsGroups.ContainsKey(type))
				{
					_cachedPrefabsGroups[type].Add(prefab);
					continue;
				}

				if (!_cachedPrefabsSingle.ContainsKey(type))
				{
					_cachedPrefabsSingle.Add(type, prefab);
					continue;
				}

				var existing = _cachedPrefabsSingle[type];
				_cachedPrefabsSingle.Remove(type);
				_cachedPrefabsGroups.Add(type, new List<BaseBehaviour> {existing, prefab});
			}
		}

		public static bool HasPrefab<T>(Predicate<T> predicate = default) where T : BaseBehaviour
		{
			var type = typeof(T);
			return _cachedPrefabsSingle.ContainsKey(type) || _cachedPrefabsGroups.ContainsKey(type);
		}

		public static T LoadPrefab<T>(Predicate<T> predicate = default) where T : BaseBehaviour
		{
			if (predicate != default) return LoadAllPrefabs<T>().Find(predicate);

			var type = typeof(T);
			return _cachedPrefabsSingle.ContainsKey(type) ? _cachedPrefabsSingle[type] as T :
			       _cachedPrefabsGroups.ContainsKey(type) ? _cachedPrefabsGroups[type][0] as T :
			                                                default(T);
		}

		public static List<T> LoadAllPrefabs<T>() where T : BaseBehaviour
		{
			var type = typeof(T);

			return _cachedPrefabsGroups.ContainsKey(type)
				       ? _cachedPrefabsGroups[type].Cast<T>().ToList()
				       : _cachedPrefabsSingle.ContainsKey(type)
					       ? new List<T> {_cachedPrefabsSingle[type] as T}
					       : new List<T>();
		}

		public static T CopyPrefab<T>(Transform parent = null,
		                              string name = "",
		                              bool activate = true) where T : BaseBehaviour
		{
			var prefab = LoadPrefab<T>();
			return prefab == null ? null : prefab.Copy(parent, name, activate);
		}

		public static T CopyPrefab<T>(Predicate<T> predicate,
		                              Transform parent = null,
		                              string name = "",
		                              bool activate = true) where T : BaseBehaviour
		{
			return LoadAllPrefabs<T>()
			       .Find(predicate)
			       ?.Copy(parent, name, activate);
		}

#if UNITY_EDITOR
		[ContextMenu("Parse prefabs")]
#if ODIN_INSPECTOR
		[Button("Parse prefabs")]
#endif

		public void ParsePrefabs()
		{
			_prefabs =
				UnityEditorTools.Find<BaseBehaviour>(Instance._prefabsPaths,
				                                     UnityEditorTools.FilterTypes.Prefab);
			_prefabs.Sort((b1, b2) => string.CompareOrdinal(b1.GetType().Name, b2.GetType().Name));

			CachePrefabs();

			EditorUtility.SetDirty(Instance);
		}
#endif
	}
}