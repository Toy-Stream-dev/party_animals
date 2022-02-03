using System;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using UnityEngine;

namespace _Idle.Scripts.Model.Numbers
{
	[Serializable]
	public class GameParam : BigNumber
	{
		public event Action LevelChanged;

		[SerializeField] private GameParamType _type;
		[SerializeField] private int _level = 1;

		public GameParamType Type => _type;
		public int Level => _level;

		public GameParam(GameParamType type)
		{
			_type = type;
		}

		public GameParam(GameParamType type, double value)
		{
			_type = type;
			Init(value);
		}

		public bool IsTrue()
		{
			return Value > 0;
		}

		public bool IsFalse()
		{
			return !IsTrue();
		}

		public void IncLevel()
		{
			_level++;
			LevelChanged?.Invoke();
		}

		public void SetLevel(int level)
		{
			_level = level;
			LevelChanged?.Invoke();
		}

		public void CopyFrom(GameParam source)
		{
			base.CopyFrom(source);

			if (_level != source._level)
			{
				_level = source._level;
				LevelChanged?.Invoke();
			}
		}

		public double GetNextLvlValue()
		{
			// var progression = GameBalance.Instance.Progressions.Find(p => p.Type == Type);
			// if (progression == null || progression.LevelCap > 0 && progression.LevelCap == Level)
			// {
			// 	return Value;
			// }
			//
			// return progression.GetValue(Level + 1);
			return 0;
		}

		public bool IsCap()
		{
			// var progression = GameBalance.Instance.Progressions.Find(p => p.Type == Type);
			// if (progression.LevelCap == 0) return false;
			// return _level >= progression.LevelCap;
			return false;
		}

		public int GetLevelCap()
		{
			// var progression = GameBalance.Instance.Progressions.Find(p => p.Type == Type);
			// return progression.LevelCap == 0 ? 0 : progression.LevelCap;
			return 0;
		}

		public override string ToString()
		{
			return $"({_type}: {Value})";
		}
	}
}