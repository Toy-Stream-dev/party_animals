using System;

namespace _Idle.Scripts.Model.Player
{
	[Serializable]
	public class AnimationEventWithCallback
	{
		public string AnimationName;
		public Action Callback;
	}
}