using GeneralTools.Tools.ExtensionMethods;
using TMPro;
using UnityEngine;

namespace GeneralTools.UI
{
	[ExecuteInEditMode]
	public class TextAutoSize : BaseBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private RectTransform _targetRect;
		[SerializeField] private Vector2 _offset, _minSize, _maxSize;
		[SerializeField, HideInInspector] private RectTransform _textRect;
		[SerializeField] private int _renderQueue;

		public string Text
		{
			get => _text.text;
			set
			{
				_text.text = value;
				_text.ForceMeshUpdate();
				UpdateSize();
				_text.ForceMeshUpdate();
			}
		}

		public TextMeshProUGUI TMPro => _text;

		private void Start()
		{
			if(_renderQueue != 0) _text.materialForRendering.renderQueue = _renderQueue;
		}

#if UNITY_EDITOR
		private void Update()
		{
			if (Application.isPlaying) return;

			if (_text == null) _text = GetComponentInChildren<TextMeshProUGUI>();
			if (_targetRect == null) _targetRect = GetComponentInChildren<RectTransform>();
			if (_textRect == null && _text != null) _textRect = _text.rectTransform;

			UpdateSize();
		}
#endif

		private bool _isEnable;
		private void OnEnable()
		{
			_isEnable = true;
		}

		private void LateUpdate()
		{
			if (_isEnable)
			{
				UpdateSize();
				_isEnable = false;
			}
		}

		public void UpdateSize()
		{
			if (_text == null || _targetRect == null) return;

			var delta = GetBackSize(_text);
			var textRect = _text.rectTransform;

			if (_targetRect == textRect.parent)
			{
				var offset = textRect.offsetMin.x.Abs() + textRect.offsetMax.x.Abs();
				delta.x += offset;
			}

			delta += _offset;

			if (_maxSize.x > 0 && delta.x > _maxSize.x) delta.x = _maxSize.x;
			else if (_minSize.x > 0 && delta.x < _minSize.x) delta.x = _minSize.x;

			if (_maxSize.y > 0 && delta.y > _maxSize.y) delta.y = _maxSize.y;
			else if (_minSize.y > 0 && delta.y < _minSize.y) delta.y = _minSize.y;

			_targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, delta.x);
			_targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, delta.y);
		}

		private Vector2 GetBackSize(TextMeshProUGUI text)
		{
			var values = _maxSize.magnitude > 0
				             ? text.GetPreferredValues(text.text, _maxSize.x, _maxSize.y)
				             : text.GetPreferredValues(text.text);
			return values.Divide(2)
			             .Ceil()
			             .Multiply(2);
		}
	}
}