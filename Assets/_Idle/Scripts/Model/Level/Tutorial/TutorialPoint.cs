using _Idle.Scripts.Balance;
using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.Model.Level.Tutorial
{
	public class TutorialPoint : BaseBehaviour
	{
		private TutorialView _view;
		
		public void SetTutorialView(TutorialView view)
		{
			_view = view;
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (((1 << other.gameObject.layer) & GameBalance.Instance.PlayerLayer) != 0)
			{
				_view.NextStep();
				gameObject.SetActive(false);
			}
		}
	}
}