using System;
using System.Collections.Generic;
using System.Linq;
using GeneralTools.Tools;
using GoogleParse;
using UnityEngine;

namespace GeneralTools.Localization
{
	[CreateAssetMenu(fileName = "LocalizationsContainer", menuName = "General tools/LocalizationsContainer", order = 1)]
	public class LocalizationsContainer : SingletonScriptableObject<LocalizationsContainer>, IParsable
	{
		public List<LocalizationData> Localization;

		public static string Get(string token, Language language)
		{
			var data = Instance.Localization.Find(d => d.Token == token);
			return data != null ? data.Translations[(int)language] : token;
		}

		public static Dictionary<string, string> GetDictionary(Language language)
		{
			var dictionary = new Dictionary<string, string>();

			var langIndex = (int)language;

			foreach (var localizationData in Instance.Localization)
			{
				if (dictionary.ContainsKey(localizationData.Token))
				{
					Debug.LogWarning($"\"{localizationData.Token}\" token already exists in localization");
					continue;
				}
				var translated = localizationData.Translations[langIndex];

				dictionary.Add(localizationData.Token, translated);
			}

			return dictionary;
		}

		public virtual void OnParsed()
		{
			var allLanguages = Enum.GetValues(typeof(Language))
			                       .Cast<int>()
			                       .ToList();

			allLanguages.Remove((int)Language.None);
			var maxIndex = allLanguages.LastValue();

			var tokens = new List<string>();
			foreach (var localizationData in Localization)
			{
				var token = localizationData.Token = localizationData.Token.ToLower();
				if (tokens.Contains(token))
				{
					Debug.LogWarning($"\"{token}\" token already exists in localization");
					continue;
				}

				tokens.Add(token);

				var translations = localizationData.Translations ??
				                   (localizationData.Translations = new List<string>(maxIndex));

				while (translations.Count != maxIndex + 1)
				{
					translations.Add(string.Empty);
				}

				if (localizationData.Token == "empty") continue;

				foreach (var languageIndex in allLanguages)
				{
					if (translations[languageIndex] == string.Empty)
					{
						translations[languageIndex] = localizationData.Token;
					}
				}
			}
		}
	}
}