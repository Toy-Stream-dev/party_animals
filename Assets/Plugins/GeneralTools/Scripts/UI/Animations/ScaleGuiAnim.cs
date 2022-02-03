using System;
using DG.Tweening;
using UnityEngine;

namespace GeneralTools.UI.Animations
{
	public class ScaleGuiAnim : CustomGuiAnim
	{
		[SerializeField] private float _showStartScale = 0.5f,
		                               _hideEndScale = 0.7f;

		protected override void PlayOpenAnim(Action callback = null)
		{
			RectTransform.localScale = Vector3.one * _showStartScale;
			RectTransform.DOKill();

			Tween = RectTransform.DOScale(Vector3.one, DurationShow)
			                     .SetEase(CurveShow)
			                     .SetUpdate(true)
			                     .OnComplete(() => callback?.Invoke());
		}

		protected override void PlayCloseAnim(Action callback = null)
		{
			RectTransform.DOKill();
			Tween = RectTransform.DOScale(Vector3.one * _hideEndScale, DurationHide)
			                     .SetEase(CurveHide)
			                     .SetUpdate(true)
			                     .OnComplete(() => callback?.Invoke());
		}

		public override void Force(GuiAnimType type)
		{
			Tween?.Kill();
			RectTransform.localScale = Vector3.one;
		}

		public ScaleGuiAnim CopyFrom(ScaleGuiAnim source)
		{
			DurationShow = source.DurationShow;
			DurationHide = source.DurationHide;

			CurveShow = source.CurveShow;
			CurveHide = source.CurveHide;

			_showStartScale = source._showStartScale;
			_hideEndScale = source._hideEndScale;

			return this;
		}
	}
}