using System;
using System.Collections.Generic;
using GoogleParse.Attributes;

namespace GeneralTools.Localization
{
	[Serializable]
	public class LocalizationData
	{
		public string Token;
		[RepetitiveTableValues(2)] public List<string> Translations;
	}
}