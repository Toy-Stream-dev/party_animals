using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace GeneralTools.UI.Animations
{
	public enum GuiAnimType
	{
		Open,
		Close
	}

	public class CustomGuiAnim : BaseBehaviour
	{

		[SerializeField] private RectTransform _target;
		[SerializeField] protected float DurationShow = 0.3f, DurationHide = 0.1f;

		[FormerlySerializedAs("СurveShow")] [SerializeField] protected AnimationCurve CurveShow = AnimationCurve.EaseInOut(0, 0, 1, 1);
		[FormerlySerializedAs("СurveHide")] [SerializeField] protected AnimationCurve CurveHide = AnimationCurve.EaseInOut(0, 0, 1, 1);

		private bool _isCached;
		private RectTransform _cachedRectTransform;

		protected Tweener Tween { get; set; }
		protected RectTransform RectTransform => _isCached ? _cachedRectTransform : CacheRectTransform();

		private RectTransform CacheRectTransform()
		{
			_cachedRectTransform = _target != null ? _target : GetComponent<RectTransform>();
			_isCached = _cachedRectTransform != null;
			return _cachedRectTransform;
		}

		public void SetTarget(RectTransform target)
		{
			_target = target;
			_cachedRectTransform = _target;
		}

		public virtual CustomGuiAnim PlayAnim(GuiAnimType type, Action callback = null)
		{
			Tween?.Kill();

			switch (type)
			{
				case GuiAnimType.Open:
					PlayOpenAnim(callback);
					break;

				case GuiAnimType.Close:
					PlayCloseAnim(callback);
					break;
			}

			return this;
		}

		public virtual CustomGuiAnim SetDelay(float delay)
		{
			Tween?.SetDelay(delay);
			return this;
		}

		protected virtual void PlayOpenAnim(Action callback = null)
		{
			callback?.Invoke();
		}

		protected virtual void PlayCloseAnim(Action callback = null)
		{
			callback?.Invoke();
		}

		public virtual void Force(GuiAnimType type)
		{
		}

		private void OnDestroy()
		{
			KillTween();
		}

		public void KillTween()
		{
			Tween?.Kill();
		}
	}
}