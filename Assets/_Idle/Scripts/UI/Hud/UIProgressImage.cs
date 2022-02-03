using GeneralTools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.Hud
{
	public class UIProgressImage : BaseUIBehaviour
	{
		[SerializeField] private Image _image;

		public void Show()
		{
			gameObject.SetActive(true);
		}
		
		public void Hide()
		{
			gameObject.SetActive(false);
		}
		
		public void Active()
		{
			_image.color = Color.white;
		}
		
		public void Deactive()
		{
			var color = new Color(0.6f, 0.6f, 0.6f, 0.8f);
			_image.color = color;
		}
	}
}