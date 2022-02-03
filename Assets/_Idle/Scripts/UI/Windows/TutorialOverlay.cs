using System;
using _Idle.Scripts.Model;
using DG.Tweening;
using GeneralTools.Model;
using GeneralTools.UI;
using Plugins.GeneralTools.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Idle.Scripts.UI.Windows
{
	public class TutorialOverlay : BaseWindow, IPointerDownHandler
	{
		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private TextMeshProUGUI _tapToStartText;
		
		private GameModel _gameModel;
		private Tween _tween;

		public Action OnClosed;
		
		public override void Init()
		{
			_gameModel = Models.Get<GameModel>();
			
			base.Init();
		}

		public override BaseUI Open()
		{
			// _gameModel.Pause();
			
			var c = _tapToStartText.color;
			c.a = 1;
			_tapToStartText.color = c; 
			_tween = _tapToStartText.DOFade(0.1f, 0.5f)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.Linear);
			
			return base.Open();
		}

		public override void Close()
		{
			// _gameModel.ShowStartOverlay();
			// _gameModel.LevelStart();
			
			_tween?.Kill();
			_tween = null;
			
			OnClosed?.Invoke();
			OnClosed = null;
			
			base.Close();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Close();
		}

		public void UpdateText(string text)
		{
			_text.SetText(text);
		}
	}
}