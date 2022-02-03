using System.Linq;
using UnityEngine;

namespace GeneralTools.Tools
{
	public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null) Init();

				return _instance;
			}
		}

		public static void Init()
		{
			_instance = Resources.Load<T>(typeof(T).Name);
		}
	}
}