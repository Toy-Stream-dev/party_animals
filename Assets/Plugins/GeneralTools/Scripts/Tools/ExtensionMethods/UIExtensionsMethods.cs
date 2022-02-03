using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GeneralTools.Tools.ExtensionMethods
{
	public static class UIExtensionsMethods
	{
		public static Image SetAlpha(this Image image, float a)
		{
			var color = image.color;
			color.a = a;
			image.color = color;

			return image;
		}

		public static TextMeshProUGUI SetAlpha(this TextMeshProUGUI text, float a)
		{
			var color = text.color;
			color.a = a;
			text.color = color;

			return text;
		}

		public static Direction GetHideDirection(this RectTransform rectTransform)
		{
			var anchorMin = rectTransform.anchorMin;

			//TODO нужно будет обработать все ситуации (не помню что это значит)
			if (anchorMin.x.EqualTo(0f))
			{
				return anchorMin.x.EqualTo(anchorMin.y) ? Direction.Down : Direction.Left;
			}

			if (anchorMin.y.EqualTo(0f)) return Direction.Down;

			if (rectTransform.anchorMax.x.EqualTo(1f)) return Direction.Right;
			if (rectTransform.anchorMax.y.EqualTo(1f)) return Direction.Up;

			return Direction.None;
		}

		public static Vector3 GetOutOffScreenPos(this RectTransform rectTransform, Direction direction = Direction.None)
		{
			if (direction == Direction.None) direction = rectTransform.GetHideDirection();
			var delta = direction.IsVertical()
				            ? rectTransform.anchoredPosition.y.Abs() + rectTransform.rect.height
				            : rectTransform.anchoredPosition.x.Abs() + rectTransform.rect.width;

			return rectTransform.localPosition + direction.ToVec3() * delta;
		}

		public static Vector2 GetOutOffScreenDelta(this RectTransform rectTransform, Direction direction = Direction.None)
		{
			if (direction == Direction.None) direction = rectTransform.GetHideDirection();

			var delta = direction.IsVertical()
				            ? rectTransform.anchoredPosition.y.Abs() + rectTransform.rect.height
				            : rectTransform.anchoredPosition.x.Abs() + rectTransform.rect.width;

			return direction.ToVec2() * delta;
		}

		public static void AddCallback(this EventTrigger eventTrigger, EventTriggerType triggerType, Action callback)
		{
			var entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
			if (entry == null)
			{
				entry = new EventTrigger.Entry {eventID = triggerType};
				eventTrigger.triggers.Add(entry);
			}

			entry.callback.AddListener(_ => callback.Invoke());
		}

		public static Tween DOShakePosition(this RectTransform transform)
		{
			transform.DOKill(true);
			return transform.DOShakePosition(0.8f, 10f, 50, 50f);
		}
	}
}