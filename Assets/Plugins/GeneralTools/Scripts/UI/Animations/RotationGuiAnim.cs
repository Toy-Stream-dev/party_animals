using System;
using DG.Tweening;
using UnityEngine;

namespace GeneralTools.UI.Animations
{
	public class RotationGuiAnim : CustomGuiAnim
	{
		[SerializeField] private float _outOffScreenRotation, _delay;

		protected override void PlayOpenAnim(Action callback = null)
		{
			RectTransform.localEulerAngles = Vector3.forward * _outOffScreenRotation;
			Tween = RectTransform.DOLocalRotate(Vector2.zero, DurationShow)
			                     .SetEase(CurveShow)
			                     .SetUpdate(true)
			                     .OnComplete(() => callback?.Invoke())
			                     .SetDelay(_delay);
		}

		public override CustomGuiAnim SetDelay(float delay)
		{
			Tween?.SetDelay(_delay + delay);
			return this;
		}
	}
}