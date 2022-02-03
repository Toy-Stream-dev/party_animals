using System;
using System.Collections.Generic;
using System.Linq;
using _Idle.Scripts.Model;
using _Idle.Scripts.Model.Numbers;
using GeneralTools.Model;
using GeneralTools.Tools.ExtensionMethods;
using UnityEngine;

namespace _Idle.Scripts.Saves
{
	public static class GameSave
	{
		private const string KEY = "snapshot",
		                     KEY_BACKUP = "snapshot_backup";

		public const float SAVE_DELAY = 3f;

		public static Snapshot LastLoaded { get; private set; }
		public static float SaveDelay { get; private set; } = SAVE_DELAY;

		public static void Update(float deltaTime)
		{
			if (SaveDelay.EqualTo(0f)) return;

			SaveDelay -= deltaTime;

			if (SaveDelay > 0f) return;

			SaveDelay = SAVE_DELAY;
			Save();
		}

		public static void Save()
		{
			var snapshot = new Snapshot()
			{
				Time = DateTime.Now,
				Game = Models.Get<GameModel>(),
			};

			Save(snapshot);
		}

		public static void Save(Snapshot snapshot)
		{
			var json = JsonUtility.ToJson(snapshot);
			PlayerPrefs.SetString(KEY, json);
			PlayerPrefs.Save();
		}

		public static bool Exists()
		{
			return PlayerPrefs.HasKey(KEY);
		}

		public static bool BackupExists()
		{
			return PlayerPrefs.HasKey(KEY_BACKUP);
		}

		public static Snapshot Load()
		{
			if (!Exists()) return null;

			var json = PlayerPrefs.GetString(KEY);
			LastLoaded = JsonUtility.FromJson<Snapshot>(json);

			return LastLoaded;
		}

		public static void DeleteSave()
		{
			PlayerPrefs.DeleteKey(KEY);
			PlayerPrefs.Save();
		}

		public static void Backup()
		{
			if (Exists())
			{
				PlayerPrefs.SetString(KEY_BACKUP, PlayerPrefs.GetString(KEY));
				PlayerPrefs.Save();
			}
		}

		public static void Restore()
		{
			if (BackupExists())
			{
				PlayerPrefs.SetString(KEY, PlayerPrefs.GetString(KEY_BACKUP));
				PlayerPrefs.Save();
			}
		}

		public static void CopyFrom(this List<GameParam> destination, List<GameParam> source)
		{
			foreach (var sourceParam in source)
			{
				var destinationParam = destination.Find(p => p.Type == sourceParam.Type);
				if (destinationParam == null)
				{
					destinationParam = new GameParam(sourceParam.Type);
					destination.Add(destinationParam);
				}

				destinationParam.CopyFrom(sourceParam);
			}
		}

		public static void CopyFrom(this List<GameProgress> destination, List<GameProgress> source)
		{
			foreach (var sourceProgress in source)
			{
				var destinationParam = destination.Find(p => p.Type == sourceProgress.Type);
				if (destinationParam == null)
				{
					destinationParam = new GameProgress(sourceProgress.Type, sourceProgress.TargetValue, sourceProgress.Looped);
					destination.Add(destinationParam);
				}

				destinationParam.CopyFrom(sourceProgress);
			}
		}
	}
}