using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GeneralTools.UI
{
	public class InteractableUIItem : BaseUIBehaviour,
	                                   IPointerEnterHandler,
	                                   IPointerExitHandler,
	                                   IPointerDownHandler,
	                                   IPointerUpHandler
	{
		private const float CLICK_TIME = 0.4f;

		private static InteractableUIItem _currentOverlapped;

		public event Action PointerDown;

		private float _downTime;

		public static bool IsOver => _currentOverlapped != null;
		public static int LastInteractionFrame { get; private set; }

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			RegisterLastInteraction();
			_currentOverlapped = this;
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			OnOverlapEnded();
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			RegisterLastInteraction();
			_downTime = Time.time;
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			RegisterLastInteraction();
			PointerDown?.Invoke();

			if (Time.time - _downTime < CLICK_TIME)
			{
				OnClick();
			}
		}

		protected virtual void OnClick()
		{
		}

		private void OnDestroy()
		{
			OnOverlapEnded();
		}

		private void OnDisable()
		{
			OnOverlapEnded();
		}

		private void OnOverlapEnded()
		{
			RegisterLastInteraction();
			if (_currentOverlapped != this) return;

			_currentOverlapped = null;
		}

		private void RegisterLastInteraction()
		{
			LastInteractionFrame = Time.frameCount;
		}
	}
}