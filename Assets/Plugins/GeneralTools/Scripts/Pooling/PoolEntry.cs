using System;
using System.Collections.Generic;
using GeneralTools.Tools;
using UnityEngine;

namespace GeneralTools.Pooling
{
	public class PoolEntry
	{
		public Type Type { get; }
		public BaseBehaviour Source { get; private set; }

		private readonly List<BaseBehaviour> _items = new List<BaseBehaviour>();
		private readonly Transform _parent;

		public int Count => _items.Count;

		public PoolEntry(Transform poolParent, Type type)
		{
			Type = type;

			_parent = new GameObject(type.Name).transform;
			_parent.SetParent(poolParent, false);
		}

		public void Spawn<T>(int count, Predicate<T> predicate = default) where T : BaseBehaviour
		{
			var prefab = Prefabs.LoadPrefab(predicate);
			if (prefab == null)
			{
				Debug.LogError($"There is no prefab of type {typeof(T)}");
				return;
			}

			Spawn(prefab, count);
		}

		public void Spawn<T>(T source, int count) where T : BaseBehaviour
		{
			Source = source;
			Spawn(source as BaseBehaviour, count);
		}

		private void Spawn(BaseBehaviour source, int count)
		{
			while (--count >= 0)
			{
				var item = source.Copy(_parent).Deactivate();
				_items.Add(item);
			}
		}

		public void Push<T>(T item) where T : BaseBehaviour
		{
			if (item == null)
			{
				Debug.LogError($"Trying to push null obj");
				return;
			}

			if (item is IPoolable poolable) poolable.OnPushedBackToPool();

			_items.Add(item.SetParent(_parent, false)
			               .Deactivate());
		}

		public T Pop<T>(Transform parent, bool resetScale = true, bool activate = true, Predicate<T> predicate = default) where T : BaseBehaviour
		{
			if (_items.Count > 0)
			{
				var itemIndex = -1;

				if (predicate != default)
				{
					for (int i = _items.Count - 1; i >= 0; i--)
					{
						if (!predicate.Invoke(_items[i] as T)) continue;
						itemIndex = i;
						break;
					}
				}
				else
				{
					itemIndex = _items.Count - 1;
				}

				if (itemIndex >= 0)
				{
					var item = _items[itemIndex];
					_items.RemoveAt(itemIndex);

					item.SetParent(parent, false);
					item.transform.localPosition = Vector3.zero;
					if (resetScale)
					{
						item.transform.localScale = Vector3.one;	
					}

					return (activate ? item.Activate() : item) as T;
				}
			}

			return predicate == default
				       ? Source != null ? Source.Copy(parent) as T :
				                          Prefabs.CopyPrefab<T>(parent, activate: activate)
				       : Prefabs.CopyPrefab<T>(predicate, parent, activate: activate);
		}
	}
}