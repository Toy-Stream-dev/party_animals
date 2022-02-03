using System;
using System.Collections.Generic;
using System.Linq;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.Model.Numbers;
using _Idle.Scripts.UI.Hud;
using _Idle.Scripts.UI.Windows;
using DG.Tweening;
using GeneralTools.Model;
using GeneralTools.Tools;
using GeneralTools.UI;
using Plugins.GeneralTools.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.HUD
{
    public class Hud : BaseWindow
    {
        [SerializeField] private TextMeshProUGUI _soft;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TextMeshProUGUI _currentLevel;
        [SerializeField] private TextMeshProUGUI _nextLevel;
        [SerializeField] private InputPanel _inputPanel;
        [SerializeField] private UnitsHUD _unitsHud;

        private List<GamePlayElementUI> _gamePlayElements;

        private Transform _hudOverlayTransform;
        private Progress _progress;
        private GameModel _gameModel;

        public Action<Vector2> OnInputValueChange = delegate {};
        public Action<PointerEventData> OnInputPointerDown = delegate { };
        public Action<PointerEventData> OnInputPointerUp = delegate { };
        public Action<PointerEventData> OnInputPointerClick = delegate { };
        
        public override void Init()
        {
            _gameModel = Models.Get<GameModel>();
            _hudOverlayTransform = GameUI.Get<HUDOverlayContainer>().transform;
            _gamePlayElements = GameUI.Root.GetComponentsInChildren<GamePlayElementUI>(true).ToList();
            _inputPanel.OnOnDrag += InputValueChange;
            _inputPanel.OnOnPointerDown += OnPointerDown;
            _inputPanel.OnOnPointerUp += OnPointerUp;
            _inputPanel.OnOnPointerClick += OnPointerClick;
            
            // HideLevelProgress();
            _gameModel.GetParam(GameParamType.Soft).UpdatedEvent += UpdateSoft;
            
            _unitsHud.StartMe();

            base.Init();
        }

        private void InputValueChange(Vector2 value)
        {
            OnInputValueChange.Invoke(value);
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            OnInputPointerDown.Invoke(eventData);
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            OnInputPointerUp.Invoke(eventData);
        }

        private void OnPointerClick(PointerEventData eventData)
        {
            OnInputPointerClick.Invoke(eventData);
        }

        public override BaseUI Open()
        {
            SetProgress();
            UpdateSoft();
            
            return base.Open();
        }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();
        }

        public override void UpdateMe(float deltaTime)
        {
            _inputPanel.UpdateMe(deltaTime);
            
            base.UpdateMe(deltaTime);
        }

        public void ClearUnits()
        {
            _unitsHud.Clear();
        }

        public void HideUnits()
        {
            _unitsHud.Deactivate();
        }

        public void ShowUnits()
        {
            _unitsHud.Activate();
        }

        public void UpdateSoft()
        {
            _soft.SetText($"{_gameModel.GetParam(GameParamType.Soft).Value}");
        }

        public void ShowLevel(int currentLevel, int nextLevel)
        {
            _currentLevel.SetText($"{currentLevel}");
            // _nextLevel.SetText(nextLevel.ToString());
        }

        public void ShowGameplayElements(params GamePlayElement[] elements)
        {
            foreach (var element in elements)
            {
                var elementUI = _gamePlayElements.Find(e => e.Element == element);
                if (elementUI == null) continue;
                elementUI.Activate();
            }
        }

        public void HideGameplayElements(params GamePlayElement[] elements)
        {
            foreach (var element in elements)
            {
                var elementUI = _gamePlayElements.Find(e => e.Element == element);
                if (elementUI == null) continue;
                elementUI.Deactivate();
            }
        }
        
        public void MoveToOverlay(params GamePlayElement[] elements)
        {
            foreach (var element in elements)
            {
                var elementUI = _gamePlayElements.Find(e => e.Element == element);
                if (elementUI == null) continue;
                if (elementUI.transform.parent == _hudOverlayTransform) continue;
                elementUI.transform.SetParent(_hudOverlayTransform, true);
                elementUI.Activate();
            }
        }
        
        public void ReturnFromOverlay(params GamePlayElement[] elements)
        {
            foreach (var element in elements)
            {
                var elementUI = _gamePlayElements.Find(e => e.Element == element);
                if (elementUI != null) elementUI.transform.SetParent(transform, true);
            }
        }
        
        public void ReturnFromOverlayAndDeactivate(params GamePlayElement[] elements)
        {
            foreach (var element in elements)
            {
                var elementUI = _gamePlayElements.Find(e => e.Element == element);
                if (elementUI != null) elementUI.transform.SetParent(transform, true);
                elementUI.Deactivate();
            }
        }
        
        public void ReturnAllBackFromOverlay()
        {
            while (_hudOverlayTransform.childCount > 0)
            {
                _hudOverlayTransform.GetChild(0).SetParent(transform, true);
            }
        }

        public void ShowLevelProgress()
        {
            // _currentLevel.gameObject.SetActive(true);
            // _progressSlider.gameObject.SetActive(true);
        }

        public void HideLevelProgress()
        {
            _currentLevel.gameObject.SetActive(false);
            _progressSlider.gameObject.SetActive(false);
        }

        public void RefreshProgress()
        {
            _progress = Models.Get<GameModel>().GetProgress(GameParamType.Progress);
            _progressSlider.maxValue = (float)_progress.TargetValue;
        }

        private void SetProgress()
        {
            _progress = Models.Get<GameModel>().GetProgress(GameParamType.Progress);
            _progressSlider.maxValue = (float)_progress.TargetValue;
            _progress.UpdatedEvent += UpdateProgress;
            // _progress.CompletedEvent += CompleteLevel;
        }

        private void UpdateProgress()
        {
            _progressSlider.value = (float)_progress.CurrentValue;
        }

        private void CompleteLevel()
        {
            GameUI.Get<LevelCompletedWindow>().Open();
        }
    }
}