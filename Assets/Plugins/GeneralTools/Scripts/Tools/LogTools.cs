using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GeneralTools.Tools
{
	public static class LogTools
	{

		private const string OPEN_CHAR = "{",
		                     CLOSE_CHAR = "}",
		                     SEPARATOR = ", ";

		public static string ToCollectionString<T>(this IEnumerable<T> collection, bool addCollectionName = true)
		{
			return ParseIfEnumerable(collection, addCollectionName) as string;
		}

		public static object ParseIfEnumerable(object message, bool addCollectionName = true)
		{
			if (message is string) return message;
			
			if (!(message is IEnumerable enumerable)) return message;

			var collectionTypeName = enumerable.GetType().Name;

			var strBuilder = new StringBuilder();
			if (addCollectionName) strBuilder.Append(collectionTypeName);

			var started = false;
			
			foreach (var obj in enumerable)
			{
				if (obj == null) continue;

				if (!started)
				{
					if (addCollectionName)
					{
						var itemsTypeName = obj.GetType().Name;
						if (!collectionTypeName.Contains(itemsTypeName))
						{
							strBuilder.Append($" of {itemsTypeName}");
						}
						strBuilder.Append($": ");
					}
					strBuilder.Append(OPEN_CHAR);
					started = true;
				}
				
				strBuilder.Append(obj);
				strBuilder.Append(SEPARATOR);
			}

			if (!started)
			{
				var arguments = enumerable.GetType().GetGenericArguments();
				var itemTypeName = arguments.Length > 0 ? arguments[0].Name : "";
				return $"Empty {collectionTypeName} of {itemTypeName}";
			}
			
			if (strBuilder.Length > 0)
			{
				strBuilder.Remove(strBuilder.Length - SEPARATOR.Length, SEPARATOR.Length);
			}

			strBuilder.Append(CLOSE_CHAR);

			return strBuilder.ToString();
		}
	}
}