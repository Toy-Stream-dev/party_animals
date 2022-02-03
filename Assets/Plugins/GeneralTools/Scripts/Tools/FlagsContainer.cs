using System;
using System.Linq;
using UnityEngine;

namespace GeneralTools.Tools
{
	[Serializable]
	public class FlagsContainer<T> where T : struct, Enum
	{
		public delegate void ChangeFlagDelegate(T flag, bool active);
		public event ChangeFlagDelegate ChangedEvent;

		private T[] _allValues;
		private int[] _allValuesInt;

		[SerializeField] private int _flagsValue;

		public T Value => (T)(object)_flagsValue;

		public FlagsContainer()
		{
			_allValues = EnumTools.GetValues<T>().ToArray();
			_allValuesInt = _allValues.Select(v => (int)(object)v).ToArray();
		}

		public void Set(T flag)
		{
			Set(flag, true);
		}

		public void Clear(T flag)
		{
			Set(flag, false);
		}

		public void Set(T flag, bool active)
		{
			var flagValue = (int)(object)flag;

			if (active == Has(flagValue)) return;

			if (active)
			{
				_flagsValue |= flagValue;
			}
			else
			{
				_flagsValue ^= flagValue;
			}

			ChangedEvent?.Invoke(flag, active);
		}

		public bool Has(int flag)
		{
			return (_flagsValue & flag) == flag;
		}

		public bool Has(T flag)
		{
			return Has((int)(object)flag);
		}
		public bool HasAny(int flag)
		{
			return (_flagsValue & flag) > 0;
		}

		public bool HasAny(T flag)
		{
			return HasAny((int)(object)flag);
		}

		public void CopyFrom(T source)
		{
			var sourceValue = (int)(object)source;

			for (var i = 0; i < _allValuesInt.Length; i++)
			{
				var flagValue = _allValuesInt[i];

				var has = Has(flagValue);
				var sourceHas = (sourceValue & flagValue) == flagValue;

				if (has == sourceHas) continue;

				if (has)
				{
					_flagsValue ^= flagValue;
				}
				else
				{
					_flagsValue |= flagValue;
				}

				ChangedEvent?.Invoke(_allValues[i], sourceHas);
			}
		}

		public void Clear()
		{
			for (var i = 0; i < _allValuesInt.Length; i++)
			{
				var flagValue = _allValuesInt[i];

				if (!Has(flagValue)) continue;

				_flagsValue ^= flagValue;
				ChangedEvent?.Invoke(_allValues[i], false);
			}
		}
	}
}