using _Idle.Scripts.View.Item;
using _Idle.Scripts.View.Unit;
using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.Model.Unit
{
    public class SpineCollider : BaseBehaviour
    {
        [SerializeField] private UnitView _unitView;
        [SerializeField] private InteractableItemView _interactableItemView;

        public UnitModel Unit => _unitView.Model;
        public InteractableItemView InteractableItemView => _interactableItemView;
    }
}