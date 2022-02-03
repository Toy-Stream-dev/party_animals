using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralTools.Tools
{
	public static class EnumTools
	{
		public static IEnumerable<T> GetValues<T>(bool excludeDefault = true) where T : Enum
		{
			var values = Enum.GetValues(typeof(T)).Cast<T>();
			if (!excludeDefault) return values;

			var defaultValue = default(T);
			return values.Where(value => !Equals(value, defaultValue));
		}
	}
}