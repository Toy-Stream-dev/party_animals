using DG.Tweening;
using GeneralTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Idle.Scripts.View.Level
{
	public enum Direction { Right, Left, Custom }
	
	public class AutoRotation : BaseBehaviour
	{
		[SerializeField] private Direction _direction;
		[ShowIf(nameof(_direction), Direction.Custom)]
		[SerializeField] private Vector3 _customDirection;
		[SerializeField] private float _speed = 0.5f;

		private void Start()
		{
			var endValue = _direction == Direction.Custom 
				? _customDirection
				: new Vector3(0, 0, _direction ==  Direction.Right ? -180 : 180);

			_speed = 1.0f / _speed;
			transform.DORotate(endValue, _speed)
				.SetEase(Ease.Linear)
				.SetLoops(-1, LoopType.Incremental);
		}
	}
}