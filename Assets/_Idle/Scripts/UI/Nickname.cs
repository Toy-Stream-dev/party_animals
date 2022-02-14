using System;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Model;
using _Idle.Scripts.Model.Unit;
using GeneralTools.Model;
using GeneralTools.Tools;
using GeneralTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI
{
	public class Nickname : BaseUIBehaviour
	{
		[SerializeField] private TextMeshProUGUI _nickname;
		[SerializeField] private Image _subImage;
		[SerializeField] private Color _playerColor;
		[SerializeField] private Color _opponentColor;
		
		public string NicknameText => _nickname.text;

		public void Init(UnitType unitType)
		{
			switch (unitType)
			{
				case UnitType.Player:
					_subImage.color = _playerColor;
					_nickname.SetText(Models.Get<GameModel>().PlayerNickname);
					break;
				default:
					_subImage.color = _opponentColor;
					_nickname.SetText(GameBalance.Instance.NickNames.RandomValue());
					break;
			}
		}
	}
}