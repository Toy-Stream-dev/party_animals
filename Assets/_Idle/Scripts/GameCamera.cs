using _Idle.Scripts.Balance;
using Cinemachine;
using DG.Tweening;
using GeneralTools;
using GeneralTools.Tools;
using UnityEngine;

namespace _Idle.Scripts
{
	public class GameCamera : BaseBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
		[SerializeField] private GameObject _fxConfetti;
		
		public static GameCamera Instance { get; private set; }
		public static Camera UnityCam { get; private set; }

		private void Awake()
		{
			Instance = this;
			UnityCam = GetComponentInChildren<Camera>();
		}

		public void Init()
		{
			_cinemachineVirtualCamera.m_Lens.FieldOfView = GameBalance.Instance.StartFOV;
		}

		public void ZoomInOut(float time)
		{
			DOTween.To(x => _cinemachineVirtualCamera.m_Lens.FieldOfView = x, GameBalance.Instance.StartFOV,
				GameBalance.Instance.SlowMotionFOV, time * 0.2f).OnComplete(() =>
			{
				DOTween.To(x => _cinemachineVirtualCamera.m_Lens.FieldOfView = x, GameBalance.Instance.SlowMotionFOV,
					GameBalance.Instance.StartFOV, time * 0.7f).SetEase(Ease.InExpo);
			});
		}

		public void Follow(Transform target, bool lookAt = true)
		{
			_cinemachineVirtualCamera.Follow = target;
		
			if (lookAt)
			{
				LookAt(target);
			}
		}

		public void LookAt(Transform target)
		{
			_cinemachineVirtualCamera.LookAt = target;
		}

		public void ShowFx()
		{
			_fxConfetti?.Activate();
		}

		public void HideFx()
		{
			_fxConfetti?.Deactivate();
		}
	}
}