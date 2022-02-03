using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeneralTools.Tools.ExtensionMethods;

namespace GeneralTools.Localization
{
	public enum Language
	{
		None = -1,
		Ru = 0,
		En = 1
	}

	public static class Localization
	{
		public static event Action LanguageChangedEvent;

		private const string VALUE_CHAR = "##",
		                     DESC = "_desc";

		private static Dictionary<string, string> _dictionary = new Dictionary<string, string>();

		public static Language CurrentLanguage { get; private set; }

		public static void ApplyLanguage(Language language)
		{
			CurrentLanguage = language;
			_dictionary = LocalizationsContainer.GetDictionary(language);
			LanguageChangedEvent?.Invoke();
		}

		public static string Localized(this string token, params object[] values)
		{
			var tokenToLower = token.ToLower();

			var localized = _dictionary.ContainsKey(tokenToLower) ? _dictionary[tokenToLower] : "";

			if (localized.IsNullOrEmpty()) localized = token;

			return values.Length == 0 ? localized : ReplaceValues(localized, values);
		}

		private static string ReplaceValues(string localized, object[] values)
		{
			var regex = new Regex(Regex.Escape(VALUE_CHAR));

			foreach (var value in values)
			{
				localized = regex.Replace(localized, value.ToString(), 1);
			}

			return localized;
		}

		public static string Localized(this Enum token, params object[] values)
		{
			return Localized(token.ToString(), values);
		}

		public static string LocalizedDescription(this Enum token, params object[] values)
		{
			return (token + DESC).Localized(values);
		}

		public static string ToDescriptionToken(this Enum token)
		{
			return token + DESC;
		}

		public static string LocalizedDescription(this string token, params object[] values)
		{
			return (token + DESC).Localized(values);
		}
	}
}