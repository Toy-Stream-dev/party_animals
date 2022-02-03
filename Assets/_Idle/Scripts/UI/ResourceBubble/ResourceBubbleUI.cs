using System;
using DG.Tweening;
using GeneralTools.Tools;
using GeneralTools.UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Idle.Scripts.UI.ResourceBubble
{
    public class ResourceBubbleUI : BaseUIBehaviour
    {
		private enum Phase
		{
			First,
			Second
		}
		
		public Action<ResourceBubbleUI> OnEventCompleted;
		
		[SerializeField] private Image _image;
		
		private Vector2 _startMovePos, 
						_middlePos, 
						_targetPos;
		
		private Vector2 _delta;

		private Phase _phase;

		private float _progress,
		              _firstPhaseDuration,
		              _secondPhaseDelay,
		              _secondPhaseDuration;

		private Tweener _tween;

		public ResourceBubbleUI SetTimings(float firstPhaseDuration,
		                                   float secondPhaseDuration,
		                                   float secondPhaseDelay)
		{
			_firstPhaseDuration = firstPhaseDuration;
			_secondPhaseDelay = secondPhaseDelay;
			_secondPhaseDuration = secondPhaseDuration;

			return this;
		}

		public ResourceBubbleUI SetPositions(Vector2 startPos, Vector2 targetPos)
		{
			_startMovePos = _middlePos = startPos;
			_targetPos = targetPos;

			_delta = Vector2.zero;

			rectTransform.position = _startMovePos;
			rectTransform.localScale = Vector3.one;
			
			this.Deactivate();

			return this;
		}

		public ResourceBubbleUI SetRange(int range)
		{
			_middlePos = _startMovePos + Random.insideUnitCircle * range;
			_delta = _middlePos - _startMovePos;
			
			return this;
		}
		
		public ResourceBubbleUI SetDelay(float delay)
		{
			_tween.SetDelay(delay);
			return this;
		}
		
		public ResourceBubbleUI Play()
		{
			_phase = Phase.First;
			StartTween(_firstPhaseDuration);
			return this;
		}
		
		public ResourceBubbleUI Redraw(BubbleTypes type, double value)
		{
			switch (type)
			{
				case BubbleTypes.Soft:
				case BubbleTypes.Hard:
					_image.sprite = $"Icon{type}".GetSprite();
					break;
			}
			
			return this;
		}
		
		private void StartTween(float duration, float delay = 0f)
		{
			_progress = 0f;
			_tween.Kill();
			_tween = DOTween.To(() => _progress, v => _progress = v, 1, duration)
			                .SetDelay(delay)
			                .SetEase(_phase == Phase.First ? Ease.OutCubic : Ease.InCubic)
			                .SetUpdate(UpdateType.Manual)
			                .OnStart(() => this.Activate())
			                .OnUpdate(OnUpdate)
			                .OnComplete(OnTweenPlayed);
		}

		private void OnCompleted()
		{
			_tween?.Kill();
			OnEventCompleted?.Invoke(this);
		}

		private void OnUpdate()
		{
			var pos = _startMovePos + _progress * _delta;
			rectTransform.position = pos;
		}

		private void OnTweenPlayed()
		{
			if (_phase == Phase.Second)
			{
				OnCompleted();
				return;
			}

			_phase = Phase.Second;

			_startMovePos = _middlePos;
			_delta = _targetPos - _startMovePos;

			StartTween(_secondPhaseDuration, _secondPhaseDelay);
		}
    }
}