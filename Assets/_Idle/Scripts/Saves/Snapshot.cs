using System;
using System.Globalization;
using _Idle.Scripts.Model;
using UnityEngine;

namespace _Idle.Scripts.Saves
{
	public class Snapshot : ISerializationCallbackReceiver
	{
		public GameModel Game;
		public DateTime Time;
		public int Version;

		[SerializeField] private string _gameStr = string.Empty;
		[SerializeField] private string _timeStr = string.Empty;
		[SerializeField] private string _versionStr = string.Empty;

		public void OnBeforeSerialize()
		{
			_gameStr = JsonUtility.ToJson(Game);
			_timeStr = Time.ToString(CultureInfo.InvariantCulture);
			_versionStr = "1";
		}

		public void OnAfterDeserialize()
		{
			Game = JsonUtility.FromJson<GameModel>(_gameStr);
			Time = DateTime.Parse(_timeStr, CultureInfo.InvariantCulture);
			
			int.TryParse(_versionStr, out var version);
			Version = version;
		}
	}
}