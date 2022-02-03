using System;
using _Idle.Scripts.Enums;
using UnityEngine;

namespace _Idle.Scripts.Model.Numbers
{
	[Serializable]
	public class GameProgress : Progress
	{
		[SerializeField] private GameParamType _type;

		public GameParamType Type => _type;

		public GameProgress(GameParamType type, BigNumber target, bool looped = true) : base(target, looped)
		{
			_type = type;
		}
		
		public GameProgress(GameParamType type, BigNumber current, BigNumber target, bool looped = true) : base(current, target, looped)
		{
			_type = type;
		}

		public GameProgress(BigNumber current, BigNumber target) : base(current, target)
		{
		}
	}
}