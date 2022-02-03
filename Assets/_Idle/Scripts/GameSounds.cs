using System;
using System.Collections.Generic;
using GeneralTools;
using GeneralTools.UI;
using Plugins.GeneralTools.Scripts.UI;
using UnityEngine;

namespace _Idle.Scripts
{
	public enum GameSoundType
	{
		None,
		ButtonClick,
		Tap,
		Pickup,
		Open,
		Close,
		Menu,
		Battle,
		SuccessPurchased,
		FailedPurchased,
		GetSoft,
		GetSoftFromAd,
		HandHit_1,
		HandHit_2,
		HandHit_3,
		MeleeWeaponHit_1,
		BombExplosion_1,
		CakeExplosion_1
	}

	[Serializable]
	public class GameSound
	{
		public GameSoundType Type;
		public AudioClip Clip;
	}

	public class GameSounds : BaseBehaviour
	{
		public static GameSounds Instance { get; private set; }

		[SerializeField] private List<GameSound> _sounds;
		[SerializeField] private AudioSource _sourceMusic;
		[SerializeField] private AudioSource _sourceSound;

		private void Awake()
		{
			Instance = this;
		}

		public void Init()
		{
			BaseButton.ClickSoundEvent += _ => PlaySound(GameSoundType.ButtonClick);
			// BaseWindow.WindowOpenedEvent += _ => PlaySound(GameSoundType.Open);
			// BaseWindow.WindowClosedEvent += _ => PlaySound(GameSoundType.Close);
			GameSettings.Instance.SoundChangedEvent += UpdateSoundsSettings;

			UpdateSoundsSettings();
		}

		private void UpdateSoundsSettings()
		{
			_sourceMusic.mute = GameSettings.Instance.IsMuteMusic;
			_sourceSound.mute = GameSettings.Instance.IsMuteSound;
		}
		
		public void PlayMusic(GameSoundType gameSoundType)
		{
			if (GameSettings.Instance.IsMuteMusic) return;
			
			var clip = _sounds.Find(s => s.Type == gameSoundType)?.Clip;
			if (clip == null) return;
			_sourceMusic.clip = clip;
			_sourceMusic.loop = true;
			_sourceMusic.time = 0f;
			_sourceMusic.Play();
		}

		public void PlaySound(GameSoundType gameSoundType, float volume = 1.0f, float pitch = 1.0f)
		{
			if (GameSettings.Instance.IsMuteSound) return;
			
			var clip = _sounds.Find(s => s.Type == gameSoundType)?.Clip;
			
			if (clip == null) 
				return;
			
			_sourceSound.volume = volume;
			_sourceSound.pitch = pitch;
			_sourceSound.PlayOneShot(clip);
		}
		
	}
}