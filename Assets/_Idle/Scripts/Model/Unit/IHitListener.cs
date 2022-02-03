namespace _Idle.Scripts.Model.Unit
{
	public interface IHitListener
	{
		void HandleCollision(CollisionArgs args);
	}
}