using GeneralTools;

namespace _Idle.Scripts.Model.Player
{
	public class AnimationEventsSender : BaseBehaviour, IAnimationEventsSender
	{
		private IAnimationEventsListener _listener;
		
		public void AssignListener(IAnimationEventsListener listener)
		{
			_listener = listener;
		}

		public void AddEvent()
		{
			//TODO: release
		}

	#region AnimationEvents
		public void ThrowUnitEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.ThrowUnit);
		}

	    public void ThrowEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.Throw);
		}
		
		public void PushEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.Push);
		}

		public void LiftingEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.Lifting);
		}
		
		public void PickupEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.Pickup);
		}
		
		public void MeleeAttackEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.MeleeWeaponAttack);
		}
		
		public void SwingMeleeWeaponEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.SwingMeleeWeapon);
		}
		
		public void EndSwingMeleeWeaponEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.EndSwingMeleeWeapon);
		}
		
		public void RangedAttackEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.RangedWeaponAttack);
		}
		
		public void RangeGetEvent()
		{
			_listener.ExecuteEvent(AnimationEventType.RangeGet);
		}

	#endregion
	}
}