using UnityEngine;

namespace _Idle.Scripts.View.Unit
{
	public class UnitBalanceController : MonoBehaviour
	{
		[SerializeField]
		private UnitView controller;
    
		//----------------------------------------
    
		void OnCollisionEnter(Collision col)
		{
			controller.PlayerGetUp();
		}
	}
}