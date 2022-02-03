using DG.Tweening;
using GeneralTools.Pooling;
using GeneralTools.Tools;
using GeneralTools.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Idle.Scripts.UI.Hud
{
    public class PopupText : BaseUIBehaviour
    {
        [SerializeField] private Vector2 _defaultPos;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Vector2 _tweenOffset = new Vector2(0, 150.0f);
        [SerializeField] private float _tweenTime = 0.5f;
        [SerializeField] private bool _scaleAnimation;
        [ShowIf(nameof(_scaleAnimation))]
        [SerializeField] private Vector3 _endScale;
        [SerializeField] private float _scaleTime = 0.4f;
        [SerializeField] private float _hideDelay = 1.0f;

        private Tween _tween;
        private Sequence _scaleSequence;
        private Vector2 _targetPosition;
        
        public PopupText SetText(string text)
        {
            _text.text = text;
			
            return this;
        }

        public PopupText PlaceAtDefaultPosition()
        {
            rectTransform.anchoredPosition = _defaultPos;
            return this;
        }

        public void Show()
        {
            _tween?.Kill();
            _tween = null;
            
            _scaleSequence?.Kill();
            _scaleSequence = null;
            
            PlaceAtDefaultPosition();

            if (_scaleAnimation)
            {
                rectTransform.localScale = Vector3.zero;

                _scaleSequence = DOTween.Sequence();
                _scaleSequence.Append(rectTransform.DOScale(_endScale, _scaleTime));
                _scaleSequence.AppendInterval(_hideDelay);
                _scaleSequence.OnComplete(Hide);
            }
            else
            {
                _targetPosition = _defaultPos + _tweenOffset;
                _tween = rectTransform.DOAnchorPos(_targetPosition, _tweenTime)
                    .OnComplete(Hide);
            }

            this.Activate();
        }

        public void Hide()
        {
            _tween?.Kill();
            _tween = null;
            
            this.PushToPool();
        }

        private void OnAnimPlayed()
        {
            this.PushToPool();
        }
    }
}