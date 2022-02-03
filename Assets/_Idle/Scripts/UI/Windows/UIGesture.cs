using System;
using System.Linq;
using DG.Tweening;
using GeneralTools.UI;
using TMPro;
using UnityEngine;

namespace _Idle.Scripts.UI.Windows
{
	public class UIGesture : BaseUIBehaviour
	{
		[SerializeField] private UIGesturePath _gesturePath;
		[SerializeField] private RectTransform _hand;
		[SerializeField] private float _tweenDuration;
		[SerializeField] private Ease _tweenEase = Ease.Linear;
		[SerializeField] private LoopType _tweenLoopType = LoopType.Restart;

		private Tween _tween;
		
		private void OnEnable()
		{
			_hand.position = _gesturePath.Path[0];
			_tween = _hand.DOPath(_gesturePath.Path, _tweenDuration)
				.SetLoops(-1, _tweenLoopType)
				.SetEase(_tweenEase);
		}

		private void OnDisable()
		{
			_tween?.Kill();
			_tween = null;
		}
	}

	[Serializable]
	public class UIGesturePath
	{
		public Transform Root;

		public Vector3[] Path
		{
			get
			{
				return Root.GetComponentsInChildren<Transform>()
					.Select(x => x.position)
					.Skip(1)
					.ToArray(); 
			}
		}
	}
}