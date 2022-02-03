using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using GeneralTools.Model;
using GeneralTools.UI;
using TMPro;
using UnityEngine;
using Random = GeneralTools.Tools.Random;

namespace _Idle.Scripts.UI.Windows
{
    public class RatingItem : BaseUI
    {
        [SerializeField] private int _spacing;
        [SerializeField] private TextMeshProUGUI _number;
        [SerializeField] private TextMeshProUGUI _nickname;
        [SerializeField] private TextMeshProUGUI _rating;
        private GameModel _game;
        private RectTransform _rect;
        private RectTransform _container;
        private RectTransform _viewport;
        private LevelCompletedWindow _window;
        private int _place;
        private int _placeUI;
        private float _sizeY;
        private int _startRating;
        public int Spacing => _spacing;
        public int RatingValue { get; private set; }

        public void Init(int place, LevelCompletedWindow window)
        {
            _game = Models.Get<GameModel>();
            _window = window;
            _rect = GetComponent<RectTransform>();
            _container = transform.parent.GetComponent<RectTransform>();
            _viewport = _container.transform.parent.GetComponent<RectTransform>();
            _sizeY = _spacing + _rect.sizeDelta.y;
            _startRating = GameBalance.Instance.StartRatingPlace;
            ResetUI(place);
            base.Init();
        }

        public override void UpdateMe(float deltaTime)
        {
            if(_viewport.anchoredPosition.y - Position() > _sizeY * 9f)
            {
                _place -= 10;
                _placeUI -= 10;
                ResetUI(_place);
                SetInvisible(_game.GetParam(GameParamType.RatingPlace).IntValue);
                MoveTo(_placeUI);
            }
            if(Position() - _viewport.anchoredPosition.y > _sizeY)
            {
                _place += 10;
                _placeUI += 10;
                ResetUI(_place);
                SetInvisible(_game.GetParam(GameParamType.RatingPlace).IntValue);
                MoveTo(_placeUI);
            }
            base.UpdateMe(deltaTime);
        }

        private float Position()
        {
            return _container.anchoredPosition.y + _rect.anchoredPosition.y;
        }

        public void ResetUI(int value, string nickname = "", int rating = 0)
        {
            _place = value;
            _number.text = $"#{_place}";
            if (nickname == "")
            {
                nickname = _window.GetNickname(_place);
            }
            _nickname.text = nickname;
            if (rating == 0)
            {
                RatingValue = (_startRating - _place) * 10 + Random.Range(0, 10);
                if (RatingValue < 0) RatingValue = 0;
            }
            else
            {
                RatingValue = rating;
            }
            _rating.text = RatingValue.ToString();
        }

        public void SetInvisible(int invisiblePlace)
        {
            if (_place == invisiblePlace)
            {
                _rect.localScale = Vector3.zero;
            }
            else
            {
                _rect.localScale = Vector3.one;
            }
        }

        public void MoveTo(int place)
        {
            _placeUI = place;
            _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, -_placeUI * _sizeY);
        }
    }
}
