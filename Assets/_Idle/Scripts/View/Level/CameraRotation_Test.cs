using Cinemachine;
using DG.Tweening;
using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.View.Level
{
	public class CameraRotation_Test : BaseBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera _camera;
		[SerializeField] private Vector3 _customDirection;
		[SerializeField] private float _speed = 0.5f;

		private void Start()
		{
			_speed = 1.0f / _speed;

			var getter = 0;
			var transposer = _camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
			
			DOTween.To(() => getter, x =>
			{
				transposer.m_XAxis.Value = x;
			}, _customDirection.y, _speed)
			.SetEase(Ease.Linear)
			.SetLoops(-1, LoopType.Incremental);
		}
	}
}