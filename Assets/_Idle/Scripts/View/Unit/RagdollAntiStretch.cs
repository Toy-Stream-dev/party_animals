using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.View.Unit
{
	public class RagdollAntiStretch : BaseBehaviour
	{
		private Vector3 _startPosition;

		private void Start()
		{
			_startPosition = transform.localPosition;
		}

		private void LateUpdate()
		{
			transform.localPosition = _startPosition;   
		}
	}
}