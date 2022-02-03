using System;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model.Unit;
using _Idle.Scripts.UI.HUD;
using _Idle.Scripts.UI.Windows;
using GeneralTools;
using GeneralTools.Model;
using GeneralTools.UI;
using UnityEngine;

namespace _Idle.Scripts.Model.Level.Tutorial
{
	public enum TutorialStep
	{
		MoveToPoint,
		TapToScreen,
		SpawnUnit,
		TutorialComplete
	}
	
	public class TutorialView : BaseBehaviour
	{
		[SerializeField] private TutorialPoint _point1;

		private GameModel _gameModel;
		private TutorialStep _currentStep;

		private void Start()
		{
			_gameModel = Models.Get<GameModel>();
			GameUI.Get<Hud>().HideGameplayElements(GamePlayElement.SoftCurrency);
			
			_point1.SetTutorialView(this);
			
			SetStep(TutorialStep.MoveToPoint);
		}

		public void NextStep()
		{
			if (_currentStep == TutorialStep.TutorialComplete)
			{
				return;
			}

			_currentStep++;
			SetStep(_currentStep);
		}

		public void SetStep(TutorialStep step)
		{
			switch (step)
			{
				case TutorialStep.MoveToPoint:
					MoveToPoint();
					break;
				case TutorialStep.TapToScreen:
					TapToScreen();
					break;
				case TutorialStep.SpawnUnit:
					SpawnUnit();
					break;
				case TutorialStep.TutorialComplete:
					TutorialComplete();
					break;
			}
			
			_currentStep = step;
		}

		private void MoveToPoint()
		{
			var tutorial = GameUI.Get<TutorialOverlay>();
			tutorial.UpdateText($"press and drag to move<br>go to the circle");
			tutorial.Open();
		}

		private void TapToScreen()
		{
			var tutorial = GameUI.Get<TutorialOverlay>();
			tutorial.UpdateText($"when you approach the enemy you start to attack<br>try to hit him to knock him out.");
			tutorial.Open();
			tutorial.OnClosed = NextStep;
		}

		private void SpawnUnit()
		{
			_gameModel.UnitsContainer.SpawnUnits();
			var units = _gameModel.UnitsContainer.GetAll();
			foreach (var unit in units)
			{
				unit.Disable();
				unit.OnFall += OnFallUnit;
				unit.OnDie += OnDieUnit;
			}
		}

		private void OnFallUnit(UnitModel unit)
		{
			_gameModel.Pause();
			var tutorial = GameUI.Get<TutorialOverlay>();
			tutorial.UpdateText($"Pick up the enemy, carry him to the edge, release your finger and you will throw him.");
			tutorial.Open();
			tutorial.OnClosed = () =>
			{
				_gameModel.Unpause();
				NextStep();
			};
		}

		private void OnDieUnit(UnitModel unit)
		{
			unit.OnFall -= OnFallUnit;
			unit.OnDie -= OnDieUnit;
			NextStep();
		}

		private void TutorialComplete()
		{
			_gameModel.Flags.Set(GameFlag.TutorialFinished, true);
		}
	}
}