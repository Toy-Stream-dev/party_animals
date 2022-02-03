using System.Collections.Generic;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Numbers;

namespace _Idle.Scripts.Model.Base
{
	public interface IModelWithParam
	{
		GameParam GetParam(GameParamType type, bool createIfNotExists = true);
		IEnumerable<GameParamType> GetCurrentParams();
	}
}