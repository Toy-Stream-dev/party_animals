using System;
using DG.Tweening;
using GeneralTools.Tools;
using GeneralTools.Tools.ExtensionMethods;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GeneralTools.UI.Animations
{
	public class PositionGuiAnim : CustomGuiAnim
	{
		[SerializeField] private Vector2 _outOffScreenPos;
		[SerializeField] private bool _recalculateOutOffScreenPos;
		[SerializeField] private Direction _direction;
		private Vector2? _defaultPos;

		public override CustomGuiAnim PlayAnim(GuiAnimType type, Action callback = null)
		{
			if (_defaultPos == null) _defaultPos = RectTransform.anchoredPosition;
			if (_recalculateOutOffScreenPos) CalculateOffscreenPos();
			return base.PlayAnim(type, callback);
		}

		protected override void PlayOpenAnim(Action callback = null)
		{
			RectTransform.anchoredPosition = _outOffScreenPos;
			Tween = RectTransform.DOAnchorPos(_defaultPos ?? Vector2.zero, DurationShow)
			                     .SetEase(CurveShow)
			                     .SetUpdate(true)
			                     .OnComplete(() => callback?.Invoke());
		}

		protected override void PlayCloseAnim(Action callback = null)
		{
			Tween = RectTransform.DOAnchorPos(_outOffScreenPos, DurationHide)
			                     .SetEase(CurveHide)
			                     .SetUpdate(true)
			                     .OnComplete(() => callback?.Invoke());
		}

		public override void Force(GuiAnimType type)
		{
			if (_defaultPos == null) _defaultPos = RectTransform.anchoredPosition;
			Tween?.Kill();
			RectTransform.anchoredPosition = type == GuiAnimType.Open ? _defaultPos.Value : _outOffScreenPos;
		}

#if ODIN_INSPECTOR
		[Button("Calculate off screen pos")]
#endif
		[ContextMenu("Calculate off screen pos")]
		public void CalculateOffscreenPos()
		{
			if (_direction == Direction.None)
			{
				_direction = RectTransform.GetHideDirection();
				if (_direction == Direction.None)
				{
					_direction = Direction.Up;
				}
			}

			var outOffScreenDelta = RectTransform.GetOutOffScreenDelta(_direction);
			_outOffScreenPos = RectTransform.anchoredPosition + outOffScreenDelta;
		}
	}
}