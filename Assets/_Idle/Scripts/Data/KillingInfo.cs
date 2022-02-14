using _Idle.Scripts.Model.Unit;

namespace _Idle.Scripts.Data
{
	public class KillingInfo
	{
		public UnitModel Killer;
		public UnitModel Victim;
		public int KillCount;

		public override string ToString()
		{
			return $"Killer is: {Killer}, Victim is: {Victim}, KillCount = {KillCount}, ";
		}
	}
}