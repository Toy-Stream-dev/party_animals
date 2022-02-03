using System;
using System.Collections.Generic;

namespace GeneralTools.Tools
{
	public static class CollectionExtensions
	{
		public static T LastValue<T>(this List<T> list)
		{
			return list.Count > 0 ? list[list.Count - 1] : default;
		}

		public static T FirstValue<T>(this List<T> list)
		{
			return list.Count > 0 ? list[0] : default;
		}

		public static List<T> Inverted<T>(this List<T> list)
		{
			var inverted = new List<T>(list.Count);

			for (int i = list.Count - 1; i >= 0; i--)
			{
				inverted.Add(list[i]);
			}

			return inverted;
		}

		public static T PopFirst<T>(this List<T> list)
		{
			if (list.Count == 0) return default(T);
			var item = list[0];
			list.RemoveAt(0);
			return item;
		}

		public static T LastValue<T>(this T[] list)
		{
			return list.Length > 0 ? list[list.Length - 1] : default(T);
		}

		public static T[] Copy<T>(this T[] sourceArray, int sourceIndex, int length)
		{
			var copy = new T[length];
			Array.Copy(sourceArray, sourceIndex, copy, 0, length);
			return copy;
		}

		public static List<T> Copy<T>(this List<T> list)
		{
			return new List<T>(list);
		}
	}
}