using System.Collections.Generic;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Model.Unit;
using DG.Tweening;
using GeneralTools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.Hud
{
	public class UnitIconUI : BaseUIBehaviour
	{
		[SerializeField] private Image _avatar;
		[SerializeField] private GameObject _cross;
		[SerializeField] private List<Image> _allImages;
		
		private Item _item;
		private UnitModel _unit;
		
		private List<Color> _defaultColor = new List<Color>();

		public void Init(UnitModel unit, Item item)
		{
			_unit = unit;
			_item = item;

			_unit.OnDie += UnitDie;

			ResetColors();
			
			_avatar.sprite = GameResources.Instance.GetSprite(_item.Identificator);
			_cross.SetActive(false);
		}

		public void UnitDie(UnitModel unit)
		{
			_unit.OnDie -= UnitDie;

			transform.DOPunchScale(Vector3.one * 1.2f, 0.3f, 0, 0)
				.OnComplete(SetGrayColors);

			_cross.SetActive(true);
		}
		
		private void Awake()
		{
			foreach (var image in _allImages)
			{
				_defaultColor.Add(image.color);
			}
		}

		private void ResetColors()
		{
			for (var i = 0; i < _allImages.Count; i++)
			{
				_allImages[i].color = _defaultColor[i];
			}
		}

		private void SetGrayColors()
		{
			foreach (var image in _allImages)
			{
				image.color = new Color(0.33f, 0.33f, 0.33f);
			}
		}
	}
}