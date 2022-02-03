using System;
using DG.Tweening;
using GeneralTools;

namespace _Idle.Scripts.View.Unit
{
	public class TargetMark : BaseBehaviour
	{
		public float MoveValue = 0.05f;
		public float MoveTime = 0.5f;
		
		private void Start()
		{
			var endY = transform.position.y - MoveValue;
			transform.DOMoveY(endY, MoveTime)
				.SetLoops(-1, LoopType.Yoyo);
		}
	}
}