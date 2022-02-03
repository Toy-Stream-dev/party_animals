using System;
using GeneralTools.Localization;
using UnityEngine;

namespace _Idle.Scripts
{
	public class GameSettings
	{
		private const string KEY = "settings";

		public event Action SoundChangedEvent, LanguageChangedEvent;
		public bool IsMuteMusic => _muteMusic;
		public bool IsMuteSound => _muteSound;
		
		private static GameSettings _instance;

		[SerializeField] private Language _language = Language.En;
		[SerializeField] private bool _muteMusic;
		[SerializeField] private bool _muteSound;

		public Language Language
		{
			get => _language;
			set
			{
				_language = value;
				Save();
				LanguageChangedEvent?.Invoke();
			}
		}

		public bool MuteMusic
		{
			get => _muteMusic;
			set => SetMuteMusic(value);
		}
		
		public bool MuteSound
		{
			get => _muteSound;
			set => SetMuteSound(value);
		}
		
		public static GameSettings Instance
		{
			get
			{
				if (_instance == null) Load();
				return _instance;
			}
		}
		
		private void SetMuteMusic(bool mute)
		{
			_muteMusic = mute;
			Save();
			SoundChangedEvent?.Invoke();
		}
		
		private void SetMuteSound(bool mute)
		{
			_muteSound = mute;
			Save();
			SoundChangedEvent?.Invoke();
		}

		public static void Load()
		{
			if (PlayerPrefs.HasKey(KEY))
			{
				var json = PlayerPrefs.GetString(KEY);
				_instance = JsonUtility.FromJson<GameSettings>(json);
			}
			else
			{
				_instance = new GameSettings
				{
					_language = Application.systemLanguage == SystemLanguage.Russian
						            ? Language.Ru
						            : Language.En
				};
			}

			if (DevFlags.FORCE_EN_LOCALIZATION)
			{
				_instance._language = Language.En;
			}
			else if (DevFlags.FORCE_RU_LOCALIZATION)
			{
				_instance._language = Language.Ru;
			}
		}

		private static void Save()
		{
			if (_instance == null) Load();
			var json = JsonUtility.ToJson(_instance);
			PlayerPrefs.SetString(KEY, json);
			PlayerPrefs.Save();
		}
	}
}