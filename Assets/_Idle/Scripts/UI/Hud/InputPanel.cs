using System;
using GeneralTools.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Idle.Scripts.UI.Hud
{
	public class InputPanel : BaseUIBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		private float _pressTime;
		
		public float DragThreshold = 0.1f;
		
		public Action<Vector2> OnOnDrag = delegate { };
		public Action<PointerEventData> OnOnPointerDown = delegate { };
		public Action<PointerEventData> OnOnPointerUp = delegate { };
		public Action<PointerEventData> OnOnPointerClick = delegate { };
		
		public void OnDrag(PointerEventData eventData)
		{
			OnOnDrag(eventData.delta);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			OnOnPointerUp(eventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_pressTime = 0.0f;
			OnOnPointerDown(eventData);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_pressTime > DragThreshold)
				return;
			
			OnOnPointerClick(eventData);
		}

		public override void UpdateMe(float deltaTime)
		{
			_pressTime += deltaTime;
			base.UpdateMe(deltaTime);
		}
	}
}