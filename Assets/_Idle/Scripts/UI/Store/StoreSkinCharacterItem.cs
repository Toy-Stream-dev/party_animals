using System;
using _Idle.Scripts.Balance;
using _Idle.Scripts.Enums;
using _Idle.Scripts.Model;
using _Idle.Scripts.UI.Message;
using _Idle.Scripts.UI.Windows;
using DG.Tweening;
using GeneralTools.Model;
using GeneralTools.Tools;
using GeneralTools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Idle.Scripts.UI.Store
{
    public class StoreSkinCharacterItem : BaseUIBehaviour
    {
        [SerializeField] private Image _avatar;
        [SerializeField] private Image _back;
        [SerializeField] private Color _newColor;
        [SerializeField] private Color _boughtColor;
        [SerializeField] private BaseButton _buyButton;

        [SerializeField] private GameObject _card;
        [SerializeField] private GameObject _empty;

        private Item _item;
        private GameModel _game;
        private bool _purchased;
        private MainWindow _window;

        private const int EMPTY_ITEM_ID = -1;

        public int Id => _item?.Id ?? EMPTY_ITEM_ID;
        public Action<bool, int> OnSkinSelected;
        
        public void Init(Item item)
        {
            _game = Models.Get<GameModel>();
            _item = item;
            _window = GameUI.Get<MainWindow>();
            
            // _buyButton.ClickedEvent += ButtonBuyPressed;
            
            _avatar.sprite = GameResources.Instance.GetSprite(_item.Identificator);
        }

        public void Redraw(bool current)
        {
            _card.Activate();
            _empty.Deactivate();
            
            SetSelected(current);

            var purchased = _game.GetParam(GameParamType.SkinsCharacter).IntValue;
            if ((purchased & 1 << _item.Id) != 0)
            {
                _purchased = true;
                _back.color = _boughtColor;
            }
            else
            {
                _back.color = _newColor;
            }
            
            // OnSkinSelected?.Invoke(_purchased, _item.Id);
        }

        public void ShowEmpty()
        {
            _card.Deactivate();
            _empty.Activate();
            // _buyButton.ClickedEvent += ButtonBuyPressed;
        }

        public void SelectItem()
        {
            if (_item == null)
            {
                GameUI.Get<MessageContainer>().Show("Coming soon.");
                OnSkinSelected?.Invoke(_purchased, EMPTY_ITEM_ID);
                return;
            }
            
            _game.Player.View.SetSkin(_item.Id);
            _game.Player.View.Victory();

            if (_purchased)
            {
                _game.SelectSkinCharacter(_item);
            }
            else
            {
                _window.ShowBuyLayer(_item);
            }
            
            OnSkinSelected?.Invoke(_purchased, _item.Id);
        }

        public void SetSelected(bool selected)
        {
            transform.DOScale(selected 
                ? Vector3.one * 1.4f
                : Vector3.one,
                0.1f);
        }
    }
}
