using GeneralTools.Localization;
using GeneralTools.UI;
using Plugins.GeneralTools.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.Windows
{
	public class SettingsWindow : BaseSoundableWindow
	{
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _musicText;
		[SerializeField] private TextMeshProUGUI _soundText;
		[SerializeField] private TextMeshProUGUI _languageText;
		
		[Space]
		[SerializeField] private BaseButton _musicButton;
		[SerializeField] private BaseButton _soundButton;
		[SerializeField] private BaseButton _englishButton;
		[SerializeField] private BaseButton _russianButton;
		
		[Space]
		[SerializeField] private string _muteText = "Mute";
		[SerializeField] private string _unmuteText = "Unmute";
		[SerializeField] private Color _muteColor;
		[SerializeField] private Color _unmuteColor;
		[SerializeField] private Image _musicButtonSub;
		[SerializeField] private Image _soundButtonSub;
		[SerializeField] private float _subImageColorRate = 0.8f;
		
		public override void Init()
		{
			// _englishButton.SetCallback(() => OnLanguageToggleChanged(Language.En));
			// _russianButton.SetCallback(() => OnLanguageToggleChanged(Language.Ru));

			_musicButton.SetCallback(OnPressedMusicButton);
			_soundButton.SetCallback(OnPressedSoundButton);

			base.Init();
		}

		public override BaseUI Open()
		{
			var settings = GameSettings.Instance;
			// _musicButton.Sprite = settings.IsMuteMusic ? "Off".GetSprite() : "On".GetSprite();
			// _soundButton.Sprite = settings.IsMuteSound ? "Off".GetSprite() : "On".GetSprite();

			_musicButton.SetText(settings.IsMuteMusic ? _unmuteText : _muteText);
			_soundButton.SetText(settings.IsMuteSound ? _unmuteText : _muteText);
			
			_musicButton.SetSpriteColor(settings.IsMuteMusic ? _muteColor : _unmuteColor);
			_soundButton.SetSpriteColor(settings.IsMuteSound ? _muteColor : _unmuteColor);

			_musicButtonSub.color = (settings.IsMuteMusic ? _muteColor : _unmuteColor) * _subImageColorRate;
			_soundButtonSub.color = (settings.IsMuteSound ? _muteColor : _unmuteColor) * _subImageColorRate;
			
			
			// RedrawLanguageButtons();
			
			return base.Open();
		}

		private void RedrawLanguageButtons()
		{
			var language = GameSettings.Instance.Language;
			switch (language)
			{
				case Language.En:
					_englishButton.SetSpriteColor(Color.white);
					_russianButton.SetSpriteColor(Color.gray);
					break;
				
				case Language.Ru:
					_englishButton.SetSpriteColor(Color.gray);
					_russianButton.SetSpriteColor(Color.white);
					break;
			}
		}

		public override void UpdateLocalization()
		{
			// _title.text = "setting".Localized();
			// _musicText.text = "music".Localized();
			// _soundText.text = "sound".Localized();
			// _languageText.text = "language".Localized();
			
			base.UpdateLocalization();
		}

		private void OnLanguageToggleChanged(Language language)
		{
			GameSettings.Instance.Language = language;
			// RedrawLanguageButtons();
		}
		
		private void OnPressedMusicButton()
		{
			var settings = GameSettings.Instance;
			settings.MuteMusic = !settings.IsMuteMusic;
			// _musicButton.SetSprite(settings.IsMuteMusic ? "Off".GetSprite() : "On".GetSprite());
			_musicButton.SetText(settings.IsMuteMusic ? _unmuteText : _muteText);
			_musicButton.SetSpriteColor(settings.IsMuteMusic ? _muteColor : _unmuteColor);
			var color = (settings.IsMuteMusic ? _muteColor : _unmuteColor) * _subImageColorRate;
			color.a = 1.0f;
			_musicButtonSub.color = color;
		}
		
		private void OnPressedSoundButton()
		{
			var settings = GameSettings.Instance;
			settings.MuteSound = !settings.IsMuteSound;
			// _soundButton.SetSprite(settings.IsMuteSound ? "Off".GetSprite() : "On".GetSprite());
			_soundButton.SetText(settings.IsMuteSound ? _unmuteText : _muteText);
			_soundButton.SetSpriteColor(settings.IsMuteSound ? _muteColor : _unmuteColor);
			var color = (settings.IsMuteSound ? _muteColor : _unmuteColor) * _subImageColorRate;
			color.a = 1.0f;
			_soundButtonSub.color = color;
		}
	}
}