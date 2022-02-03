using _Idle.Scripts.Enums;
using GeneralTools.UI;
using UnityEngine;

namespace _Idle.Scripts.UI
{
	public class GamePlayElementUI : BaseUIBehaviour
	{
		[SerializeField] private GamePlayElement _element;

		public GamePlayElement Element => _element;
	}
}