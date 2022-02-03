using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.View.Item
{
	public class RadialProgress : BaseBehaviour
	{
		[SerializeField] private SpriteRenderer _progressFill;
		
		private float _fullTime;
		private float _fillFactor;
		private int _materialPropertyId;

		public override void StartMe()
		{
			_materialPropertyId = Shader.PropertyToID("_Arc1");
			Hide();
		}

		public void SetFullTime(float fullTime)
		{
			_fullTime = fullTime;
			_fillFactor = 360.0f / fullTime;
		}

		public void UpdateProgress(float progress)
		{
			_progressFill.material.SetFloat(_materialPropertyId, 360.0f - _fillFactor * progress);
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
			
			UpdateProgress(0);
		}
	}
}