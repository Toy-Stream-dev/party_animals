using System;
using System.Collections.Generic;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.Model.Numbers;
using GeneralTools.Model;
using GeneralTools.UI;
using UnityEngine;

namespace _Idle.Scripts.UI.Hud
{
	public class UIProgressPanel : BaseUIBehaviour
	{
		[SerializeField] private List<UIProgressImage> _progressImages;

		private Progress _progress;
		
		public override void StartMe()
		{
			_progress = Models.Get<GameModel>().GetProgress(GameParamType.Progress);
			_progress.UpdatedEvent += UpdateProgress;
		}

		public void RefreshProgress()
		{
			_progress = Models.Get<GameModel>().GetProgress(GameParamType.Progress);
			var targetValue = (int) _progress.TargetValue;

			for (var i = 0; i < _progressImages.Count; i++)
			{
				var image = _progressImages[i];
				if (i >= targetValue)
				{
					image.Hide();
					continue;
				}
				
				image.Show();
				image.Deactive();
			}
		}

		private void UpdateProgress()
		{
			var val = Mathf.CeilToInt((float)_progress.CurrentValue);
			if (val > 0)
			{
				val--;
			}
			
			_progressImages[val].Active();
		}
	}
}