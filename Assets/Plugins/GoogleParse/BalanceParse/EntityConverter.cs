using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace GoogleParse.BalanceParse
{
	public static class EntityConverter
	{
		private static readonly CultureInfo _cultureInfo = new CultureInfo("en-US");

		private const string NONE = "none",
		                     EXP = "E+";

		public static object Convert(Type type, string value)
		{
			if (type == typeof(string)) return value ?? string.Empty;

			if (string.IsNullOrEmpty(value))
			{
				if (type.IsEnum)
				{
					foreach (var v in Enum.GetValues(type))
					{
						if (v.ToString().ToLower() == NONE) return Enum.Parse(type, NONE, true);
					}
				}

				return GetDefaultValue(type);
			}

			if (!type.IsValueType && type.GetConstructors()
			                             .Any(c => c.GetParameters().Length == 1 &&
			                                       c.GetParameters()[0].ParameterType == typeof(string)))
			{
				return Activator.CreateInstance(type, value);
			}

			try
			{
				if (type == typeof(bool)) return bool.Parse(value);
				if (type.IsEnum) return Enum.Parse(type, value, true);

				if (TryParseFloat(value, out var numericValue))
				{
					return type.IsValueType && type.IsPrimitive
						       ? System.Convert.ChangeType(numericValue, type)
						       : Activator.CreateInstance(type, numericValue);
				}

				return OnError();

			}
			catch (Exception e)
			{
				return OnError();
			}

			object OnError()
			{
				var defaultValue = GetDefaultValue(type);
				Debug.LogError($"Cant convert {value} to {type}, returning default value ({defaultValue})");
				return defaultValue;
			}
		}

		private static object GetDefaultValue(Type type)
		{
			try
			{
				return Activator.CreateInstance(type);
			}
			catch (Exception e)
			{
				Debug.LogError($"{e}");
			}

			return default;
		}

		private static bool TryParseFloat(string str, out float value)
		{
			return float.TryParse(str.Replace(",", ".")
		                           .Replace(" ", "")
		                           .Replace(" ", ""), //это реально два разных пробела
		                        NumberStyles.Float | NumberStyles.AllowThousands,
		                        _cultureInfo,
		                        out value);
		}
	}
}