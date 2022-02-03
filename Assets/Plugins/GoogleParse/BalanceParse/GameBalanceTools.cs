using System;
using System.Collections.Generic;
using System.Reflection;
using GoogleParse.Attributes;

namespace GoogleParse.BalanceParse
{
	[Serializable]
	public class ConstantsPair
	{
		public string key;
		public string value;
	}

	[Serializable]
	public class StringFloatPair
	{
		public string key;
		public float value;
	}

	[Serializable]
	public class ByteFloatPair
	{
		public byte key;
		public float value;
	}

	public static class GameBalanceTools
	{
		public static void ApplyValuesToObject(this List<StringFloatPair> values, object obj)
		{
			foreach (var pair in values)
			{
				ApplyValuesToObject(obj, pair.key, pair.value.ToString());
			}
		}
		public static void ApplyValuesToObject(this List<ConstantsPair> values, object obj)
		{
			foreach (var pair in values)
			{
				ApplyValuesToObject(obj, pair.key, pair.value);
			}
		}

		private static void ApplyValuesToObject(object obj, string name, string strValue)
		{
			if (string.IsNullOrEmpty(name)) return;

			var type = obj.GetType();

			var fieldInfo = type.GetField(name, BindingFlags.Public |
			                                    BindingFlags.NonPublic |
			                                    BindingFlags.Instance);

			if (fieldInfo == null) return;

			var value = EntityConverter.Convert(fieldInfo.FieldType, strValue);
			fieldInfo.SetValue(obj, value);
		}

		public static Dictionary<string, float> ToDictionary(this List<StringFloatPair> values)
		{
			var dictionary = new Dictionary<string, float>();

			foreach (var pair in values)
			{
				if (dictionary.ContainsKey(pair.key)) continue;
				dictionary.Add(pair.key, pair.value);
			}

			return dictionary;
		}

		public static Dictionary<byte, float> ToDictionary(this List<ByteFloatPair> values)
		{
			var dictionary = new Dictionary<byte, float>();

			foreach (var pair in values)
			{
				if (dictionary.ContainsKey(pair.key)) continue;
				dictionary.Add(pair.key, pair.value);
			}

			return dictionary;
		}

		public static float[] ToFloatArray(this List<ByteFloatPair> values)
		{
			var size = values[0].key == 0 ? values.Count : values.Count + 1;
			var array = new float[size];
			array[0] = 0f;

			foreach (var pair in values)
			{
				array[pair.key] = pair.value;
			}

			return array;
		}
	}
}