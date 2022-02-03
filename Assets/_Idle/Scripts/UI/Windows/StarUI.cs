using DG.Tweening;
using GeneralTools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.Windows
{
	public class StarUI : BaseUIBehaviour
	{
		[SerializeField] private Image _image;
		[SerializeField] private Sprite _fillStarSprite;
		[SerializeField] private ParticleSystem _particle;
		
		private Sprite _defaultSprite;

		public override void StartMe()
		{
			_defaultSprite = _image.sprite;
		}

		public void Disable()
		{
			_image.sprite = _defaultSprite;
		}

		public void Enable()
		{
			_image.sprite = _fillStarSprite;
			PlayVfx();
		}

		private void PlayVfx()
		{
			//TODO: release it
			// _particle?.Play();
		}
	}
}