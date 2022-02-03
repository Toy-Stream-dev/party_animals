using System;
using System.Collections.Generic;
using GeneralTools.Tools;
using UnityEngine;

namespace GeneralTools.Pooling
{
	public static class Pool
	{
		private static Transform _parent;
		private static readonly List<PoolEntry> _entries = new List<PoolEntry>();

		public static void Init(Transform root)
		{
			_parent = new GameObject("~Pool").transform;
			_parent.SetParent(root, false);
			_parent.Deactivate();
		}

		public static void Spawn<T>(int count, Predicate<T> predicate = default) where T : BaseBehaviour
		{
			GetPoolEntry<T>(true).Spawn(count, predicate);
		}

		public static void Spawn<T>(T obj, int count = 1) where T : BaseBehaviour
		{
			GetPoolEntry<T>(true).Spawn(obj, count);
		}

		public static T Pop<T>(Transform parent, bool resetScale = true, bool activate = true, Predicate<T> predicate = default) where T : BaseBehaviour
		{
			return GetPoolEntry<T>(true).Pop(parent, resetScale, activate, predicate);
		}

		public static T Pop<T>(this T behaviour) where T : BaseBehaviour
		{
			return GetPoolEntry<T>(false).Pop<T>(behaviour.transform.parent);
		}

		public static void PushToPool<T>(this T item) where T : BaseBehaviour
		{
			GetPoolEntry<T>(true).Push(item);
		}

		public static void PushAllToPoolAndClear<T>(this List<T> list) where T : BaseBehaviour
		{
			var entry = GetPoolEntry<T>(true);

			foreach (var item in list)
			{
				entry.Push(item);
			}

			list.Clear();
		}
		
		private static PoolEntry GetPoolEntry<T>(bool createIfNotExists) where T : BaseBehaviour
		{
			var type = typeof(T);
			var entry = _entries.Find(e => e.Type == type);

			if (entry == null && createIfNotExists)
			{
				entry = new PoolEntry(_parent, type);
				_entries.Add(entry);
			}

			return entry;
		}

		public static int GetPoolCount<T>() where T : BaseBehaviour
		{
			var type = typeof(T);
			return _entries.Find(e => e.Type == type)?.Count ?? 0; 
		}
	}
}