namespace _Idle.Scripts.Model.Player
{
	public interface IAnimationEventsSender
	{
		void AssignListener(IAnimationEventsListener listener);
		void AddEvent();
	}
}