using System;
using GeneralTools.Interfaces;
using GeneralTools.Localization;
using GeneralTools.Tools.ExtensionMethods;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GeneralTools.UI
{
	[ExecuteInEditMode]
	public class BaseButton : InteractableUIItem, IInteractable
	{
		public static event Action<int> ClickSoundEvent = delegate { };
		public static event Action AnyButtonClickedEvent = delegate { };
		public event Action ClickedEvent, InteractableChangedEvent;
		
		private const float ALLOWED_DRAG = 10;
		[SerializeField] protected Image _image;
		[SerializeField] private Button _button;
		[SerializeField] private bool _ignoreTransparentPixel;
		[SerializeField] protected TextMeshProUGUI _text;
		[SerializeField] private TextAutoSize _textAutoSize;
		
		[SerializeField] private string _localizationToken;
		[SerializeField] private int _clickSound = -1;

		private Texture2D _imageTexture2D;
		private Vector2 _pointerDownPos;

		public Button UnityButton => _button;
		
		public KeyCode HotKey { get; set; }

		public Action Callback { get; set; }

		public Action OnEnter { get; set; }
		public Action OnExit { get; set; }
		
		public Action OnDown { get; set; }

		public static BaseButton AllowedTutorialButton { get; set; }

		public (Action, KeyCode) ListenerAndHotKey
		{
			set
			{
				Callback = value.Item1;
				HotKey = value.Item2;
			}
		}

		public virtual string Text
		{
			get => _textAutoSize != null ? _textAutoSize.Text : _text != null ? _text.text : null;

			set
			{
				if (_textAutoSize != null) _textAutoSize.Text = value;
				else if (_text != null) _text.text = value;
			}
		}

		//TODO скрыть подобные свойства, оставив Set методы
		public Sprite Sprite
		{
			get => _image.sprite;
			set => _image.sprite = value;
		}

		public bool Interactable => _button.interactable;

		public bool Raycastable
		{
			get => _image.raycastTarget;
			set => _image.raycastTarget = value;
		}

		public bool Enabled
		{
			get => _button.enabled;
			set => _button.enabled = false;
		}

		public bool SkipUnityButtonClickCallback { get; set; }

		public float Alpha
		{
			get => _image.color.a;
			set
			{
				var imageColor = _image.color;
				imageColor.a = value;
				_image.color = imageColor;

				if (_text == null) return;

				var textColor = _text.color;
				textColor.a = value;
				_text.color = textColor;
			}
		}

#if UNITY_EDITOR
		protected void Awake()
		{
			if (Application.isPlaying) return;

			if (_image == null) _image = GetComponent<Image>();
			if (_button == null) _button = GetComponentInChildren<Button>();
			if (_textAutoSize == null) _textAutoSize = GetComponentInChildren<TextAutoSize>();
			if (_text == null) _text = GetComponentInChildren<TextMeshProUGUI>();
		}
#endif

		public BaseButton UpdateLocalization()
		{
			if (_localizationToken.IsNullOrEmpty()) return this;

			Text = _localizationToken.Localized();
			// if (localized != _localizationToken && !_localizationToken.Contains(" "))
			// {
			//	Text = localized;
			//}
			return this;
		}

		public BaseButton SetText(string text)
		{
			Text = text.Localized();
			return this;
		}

		public BaseButton SetText(Token text)
		{
			Text = text.Localized();
			return this;
		}

		public BaseButton SetText(Enum text)
		{
			Text = text.Localized();
			return this;
		}

		public BaseButton SetSprite(Sprite sprite)
		{
			_image.sprite = sprite;
			return this;
		}

		public BaseButton SetSpriteColor(Color color)
		{
			_image.color = color;
			return this;
		}

		public BaseButton SetCallback(Action callback)
		{
			Callback = callback;
			return this;
		}

		public BaseButton SetHotKey(KeyCode keyCode)
		{
			HotKey = keyCode;
			return this;
		}

		public virtual void SetInteractable(bool interactable)
		{
			_button.interactable = interactable;
			InteractableChangedEvent?.Invoke();
		}

		public void SetTextColor(Color color)
		{
			if (_text != null) _text.color = color;
		}

		public override void UpdateMe(float deltaTime)
		{
			base.UpdateMe(deltaTime);
			
			CheckHotKey();
		}

		private bool IsTransparentPixel()
		{
			if (!_image.mainTexture.isReadable)
			{
				Debug.LogError($"Image: {name} is not readable");
				return false;
			}
			
			if (_imageTexture2D == null) _imageTexture2D = (Texture2D)_image.mainTexture;
			
			var mouse = EventSystem.current.currentInputModule.input.mousePosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,mouse , null, out var position);

			var rect = rectTransform.rect;
			var px = position.x / rect.width;
			var py = position.y / rect.height;
			var tx = px * _imageTexture2D.width;
			var ty = py * _imageTexture2D.height;
			var pixel = _imageTexture2D.GetPixel((int) tx, (int) ty);
			return pixel.a <= 0;
		}

		protected void OnEnable()
		{
			if (_button == null) _button = GetComponent<Button>();

			if (_button != null)
			{
				_button.onClick.RemoveAllListeners();
				_button.onClick.AddListener(OnClick);
			}

			SkipUnityButtonClickCallback = false;
		}

		public void CheckHotKey()
		{
			if (Input.GetKeyDown(HotKey) && Interactable) Callback?.Invoke();
		}

		public virtual void Init()
		{
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			_pointerDownPos = Input.mousePosition;

			if (Interactable) OnDown?.Invoke();

			base.OnPointerDown(eventData);
		}

		public void SimulateClick()
		{
			OnClick();
		}

		private void OnUnityButtonClick()
		{
			if (SkipUnityButtonClickCallback)
			{
				SkipUnityButtonClickCallback = false;
				return;
			}

			OnClick();
		}

		protected virtual void OnClick()
		{
			if (_ignoreTransparentPixel && IsTransparentPixel()) return;
			//if (((Vector2)Input.mousePosition - _pointerDownPos).magnitude > ALLOWED_DRAG) return;
			if (!Interactable) return;

			if (AllowedTutorialButton != null)
			{
				if (AllowedTutorialButton != this) return;
				AllowedTutorialButton = null;
			}

			Callback?.Invoke();
			ClickedEvent?.Invoke();
			AnyButtonClickedEvent?.Invoke();

			if (_clickSound != -1) ClickSoundEvent?.Invoke(_clickSound);
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (Interactable) OnEnter?.Invoke();
			base.OnPointerEnter(eventData);
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			if (Interactable) OnExit?.Invoke();
			base.OnPointerExit(eventData);
		}
	}
} 