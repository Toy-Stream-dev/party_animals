using DG.Tweening;
using GeneralTools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI
{
	[RequireComponent(typeof(Graphic))]
	public class GraphicElementBlinker : BaseUIBehaviour
	{
		[Header("Target element")]
		[SerializeField] private Graphic _target;
		
		[Header("Tween settings")]
		[SerializeField] private float _startValue = 1.0f;
		[SerializeField] private float _endValue = 0.1f;
		[SerializeField] private float _duration = 0.5f;
		[SerializeField] private Ease _tweenEase = Ease.Linear;

		private Tween _tween;
		
		private void OnEnable()
		{
			var c = _target.color;
			c.a = _startValue;
			_target.color = c; 
			_tween = _target.DOFade(_endValue, _duration)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(_tweenEase);
		}

		private void OnDisable()
		{
			_tween?.Kill();
			_tween = null;
		}
	}
}