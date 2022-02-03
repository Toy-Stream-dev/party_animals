using System;
using System.Collections.Generic;
using DG.Tweening;
using GeneralTools.Tools;
using GeneralTools.Tools.ExtensionMethods;
using GeneralTools.UI.Animations;
using UnityEngine;

namespace GeneralTools.UI
{
	public class BaseUIBehaviour : BaseBehaviour
	{
		private enum State
		{
			None,
			Opened,
			Closed,
			PlayingAnim
		}

		public static float PositionAnimTime { get; set; } = 0.2f;
		public static float ScaleAnimTime { get; set; } = 0.1f;
		public static float RotationAnimTime { get; set; } = 0.1f;

		private RectTransform _cachedRectTransform;
		private bool _cached;
		private Vector2 _startPos;
		private Tweener _posTween, _scaleTween, _rotationTween;
		private State _viewState;
		private State ViewState
		{
			get => _viewState;
			set
			{
				var stateChanged = _viewState != value;
				_viewState = value;
				if (!stateChanged) return;
				switch (_viewState)
				{
					case State.Opened:
						OnOpened();
						break;
					case State.Closed:
						OnClosed();
						break;
					case State.PlayingAnim:
						OnAnimStarted();
						break;
				}
			}
		}

		protected List<CustomGuiAnim> CustomAnims { get; } = new List<CustomGuiAnim>();
		public bool IsPlayingAnim => _viewState == State.PlayingAnim;
		
		public RectTransform rectTransform => _cached ? _cachedRectTransform : CacheInfo();

		public int SiblingIndex => rectTransform.GetSiblingIndex();

#if BADUMS
		private void Start()
		{
			transform.localEulerAngles = Vector3.forward * Random.Range(-10, 10);
		}
#endif
		
		public Vector2 StartPos
		{
			get
			{
				if (!_cached) CacheInfo();
				return _startPos;
			}
		}

		public Vector2 LocalPosition
		{
			get
			{
				if (!_cached) CacheInfo();
				return _cachedRectTransform.localPosition;
			}
		}

		protected RectTransform CacheInfo()
		{
			if (_cached) return _cachedRectTransform;

			_cachedRectTransform = GetComponent<RectTransform>();
			if (_cachedRectTransform != null)
			{
				_startPos = _cachedRectTransform.anchoredPosition;
			}

			CustomAnims.AddRange(GetComponentsInChildren<CustomGuiAnim>());
			_cached = true;

			return _cachedRectTransform;
		}

		public BaseUIBehaviour SetSize(float x, float y, float scale = 1f)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x * scale);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y * scale);
			return this;
		}

		public BaseUIBehaviour SetSize(Vector2 size, float scale = 1f)
		{
			SetSize(size.x, size.y, scale);
			return this;
		}
		
		public Vector2 GetSize()
		{
			return rectTransform.sizeDelta;
		}
		
		public BaseUIBehaviour SetScale(float scale = 1f)
		{
			SetSize(GetSize(), scale);
			return this;
		}
		
		
		[ContextMenu("SetAsFirstSibling")]
		public void SetAsFirstSibling()
		{
			rectTransform.SetAsFirstSibling();
		}
		
		[ContextMenu("SetAsLastSibling")]
		public void SetAsLastSibling()
		{
			rectTransform.SetAsLastSibling();
		}

		public void SetSiblingIndex(int index)
		{
			rectTransform.SetSiblingIndex(index);
		}

		public BaseUIBehaviour SetPosition(float x, float y, float scale = 1f)
		{
			rectTransform.localPosition = new Vector2(x * scale, y * scale);
			return this;
		}

		public virtual void PlayShow(Action callback = null, float delay = 0f)
		{
			PlayAnim(GuiAnimType.Open, callback, delay);
		}

		public virtual void PlayHide(Action callback = null, float delay = 0f)
		{
			PlayAnim(GuiAnimType.Close, callback, delay);
		}

		protected virtual void PlayAnim(GuiAnimType animType, Action callback = null, float delay = 0f)
		{
			var nextState = animType == GuiAnimType.Open ? State.Opened : State.Closed;
			if (nextState == ViewState)
			{
				callback?.Invoke();
				return;
			}

			if (CustomAnims.Count == 0 && !_cached) CacheInfo();

			if (CustomAnims.Count == 0)
			{
				this.SetActive(animType == GuiAnimType.Open);
				callback?.Invoke();
				OnAnimPlayed(animType);
			}
			else
			{
				if (animType == GuiAnimType.Open) this.Activate();
				ViewState = State.PlayingAnim;

				Action onPlayed = () =>
				                  {
					                  if (animType == GuiAnimType.Close) this.Deactivate();
					                  callback?.Invoke();
					                  OnAnimPlayed(animType);
				                  };

				foreach (var customAnim in CustomAnims)
				{
					customAnim.PlayAnim(animType, onPlayed)
					          .SetDelay(delay);
					onPlayed = null;
				}
			}
		}

		protected void OnAnimPlayed(GuiAnimType anim)
		{
			ViewState = anim == GuiAnimType.Open ? State.Opened : State.Closed;
		}

		protected virtual void OnAnimStarted()
		{

		}

		protected virtual void OnOpened()
		{
			//Debug.Log($"{GetType()} opened", this);
		}

		protected virtual void OnClosed()
		{
			//Debug.Log($"{GetType()} closed", this);
		}

		protected virtual void ForceAnim(GuiAnimType animType, Action callback = null)
		{
			callback?.Invoke();
			this.SetActive(animType == GuiAnimType.Open);
			OnAnimPlayed(animType);

			if (!_cached) CacheInfo();
			if (CustomAnims.Count == 0) return;

			foreach (var customAnim in CustomAnims) customAnim.Force(animType);
		}

		public Tweener AnimateScale(float value, float duration, Ease? ease = Ease.InCubic, float? delay = null, Action onComplete = null)
		{
			_scaleTween?.Kill();
			_scaleTween = rectTransform.DOScale(value * Vector3.one, duration);
			return CustomizeTween(_scaleTween, ease, delay, onComplete);
		}

		public Tweener AnimatePosition(Vector2 targetPos, float duration, Ease ease = Ease.InCubic, Action onComplete = null)
		{
			if ((targetPos - rectTransform.anchoredPosition).magnitude.EqualTo(0f))
			{
				onComplete?.Invoke();
				return null;
			}

			_posTween?.Kill();
			_posTween = rectTransform.DOAnchorPos(targetPos, duration);
			return CustomizeTween(_posTween, ease, null, onComplete);
		}

		public Tweener AnimateRotation(float value, float duration, Ease? ease = Ease.OutCubic, float? delay = null, Action onComplete = null)
		{
			_rotationTween?.Kill();
			_rotationTween = rectTransform.DOLocalRotate(Vector3.forward * value, duration);
			return CustomizeTween(_rotationTween, ease, delay, onComplete);
		}

		private Tweener CustomizeTween(Tweener tween,
		                               Ease? ease = Ease.InCubic,
		                               float? delay = 0f,
		                               Action onComplete = null)
		{
			tween.SetUpdate(UpdateType.Manual);

			if (delay.HasValue) tween.SetDelay(delay.Value);
			if (ease.HasValue) tween.SetEase(ease.Value);

			tween.OnComplete(() =>
			                 {
				                 onComplete?.Invoke();
				                 if (_posTween == tween) _posTween = null;
				                 if (_scaleTween == tween) _scaleTween = null;
				                 if (_rotationTween == tween) _rotationTween = null;
			                 });

			return tween;
		}

		public void KillAnimTweens()
		{
			foreach (var customAnim in CustomAnims)
			{
				customAnim.KillTween();
			}
		}
	}
}