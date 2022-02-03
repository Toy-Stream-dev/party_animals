using _Idle.Scripts.Model;
using GeneralTools.Model;
using GeneralTools.UI;
using UnityEngine;

namespace _Idle.Scripts.UI.SafeArea
{
	[ExecuteInEditMode]
	public class SafeArea : BaseUIBehaviour
	{
		private RectTransform _panel;
		private Rect _lastSafeArea;

		[SerializeField] private bool _x = true;
		[SerializeField] private bool _y = true;
		[SerializeField] private bool _left;
		[SerializeField] private bool _right;
		[SerializeField] private bool _bottom;
		[SerializeField] private bool _top;
		private float _bottomOffset;

		public static DeviceType Sim = DeviceType.None;

		private void Awake()
		{
			_panel = GetComponent<RectTransform>();

			if (_panel == null)
			{
				Debug.LogError("Cannot apply safe area - no RectTransform found on " + name);
			}

			var pos = new Vector2();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, new Vector2(0, Screen.height - 168), null, out pos);
			_bottomOffset = 168f / Screen.height * rectTransform.rect.height;
		}

		private void Update()
		{
			var safeArea = GetSafeArea();

#if !UNITY_EDITOR
			if (safeArea != _lastSafeArea)
#endif
			ApplySafeArea(safeArea);
		}

		private Rect GetSafeArea()
		{
			return
#if UNITY_EDITOR
			SafeAreaData.Get(Sim);
#else
			Screen.safeArea;
#endif
		}
		
		public void UpdateOffset()
		{
			var gameModel = Models.Get<GameModel>();
			if (_panel == null)
			{
				_panel = GetComponent<RectTransform>();
			}
			
			_panel.offsetMin = new Vector2(0, gameModel.Flags.Has(GameFlag.AllAdsRemoved) ? 0 : _bottomOffset);
		}

		private void ApplySafeArea(Rect rect)
		{
			var anchorMinBase = Vector2.zero;
			var anchorMaxBase = Vector2.one;

			if (!_x)
			{
				rect.x = 0;
				rect.width = Screen.width;
			}

			if (!_y)
			{
				rect.y = 0;
				rect.height = Screen.height;
			}

			var anchorMin = rect.position;
			var anchorMax = rect.position + rect.size;

			anchorMin.x = _left ? anchorMin.x / Screen.width : anchorMinBase.x;
			anchorMax.x = _right ? anchorMax.x / Screen.width : anchorMaxBase.x;

			anchorMin.y = _bottom ? anchorMin.y / Screen.height : anchorMinBase.y;
			anchorMax.y = _top ? anchorMax.y / Screen.height : anchorMaxBase.y;

			_panel.anchorMin = anchorMin;
			_panel.anchorMax = anchorMax;
		}
	}
}