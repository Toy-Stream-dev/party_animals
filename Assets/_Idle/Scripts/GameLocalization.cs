using System;
using _Idle.Scripts.Enums;
using GeneralTools.Localization;

namespace _Idle.Scripts
{
	public static class GameLocalization
	{
		public static string Localized(this Enum token, params object[] values)
		{
			return GetToken(token).Localized(values);
		}

		public static string LocalizedDescription(this Enum token, params object[] values)
		{
			return $"{GetToken(token)}_desc".Localized(values);
		}

		private static string GetToken(this Enum token, params object[] values)
		{
			switch (token)
			{
				case GameParamType gameParam:
					return $"param_{gameParam}";
			}

			return token.ToString();
		}
	}
}