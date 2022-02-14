using GeneralTools;

namespace _Idle.Scripts.View.Level
{
	public enum SpawnPointType
	{
		Weapon,
		Unit
	}
	
	public class SpawnPoint : BaseBehaviour
	{
		public SpawnPointType PointType;

		public bool Available { get; set; } = true;
	}
}