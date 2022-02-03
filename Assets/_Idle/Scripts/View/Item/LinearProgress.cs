using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.View.Item
{
	public class LinearProgress : BaseBehaviour
	{
		[SerializeField] private SpriteRenderer _progressFill;
		
		private float _fullTime;
		private float _fillFactor;
		private Vector3 _scale;

		public override void StartMe()
		{
			_scale = _progressFill.transform.localScale;
			Hide();
		}

		public void SetFullTime(float fullTime)
		{
			_fullTime = fullTime;
			_fillFactor = 1.0f / fullTime;
		}

		public void UpdateProgress(float progress)
		{
			_scale.x = Mathf.Clamp01(progress * _fillFactor);
			_progressFill.transform.localScale = _scale;
		}

		public void UpdateProgress(double progress)
		{
			UpdateProgress((float)progress);
		}

		public void UpdateRotation(Quaternion rotation)
		{
			gameObject.transform.rotation = rotation;
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
			
			_scale.x = 0.0f;
			UpdateProgress(0);
		}
	}
}