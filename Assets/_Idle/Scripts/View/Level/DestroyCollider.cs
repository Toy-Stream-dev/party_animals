using _Idle.Scripts.Balance;
using _Idle.Scripts.View.Item;
using _Idle.Scripts.View.Unit;
using GeneralTools;
using UnityEngine;

namespace _Idle.Scripts.View.Level
{
    public class DestroyCollider : BaseBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & GameBalance.Instance.PlayerLayer) != 0)
            {
                var unit = other.gameObject.GetComponent<UnitView>();
                if (unit != null)
                {
                    unit.Model.Kill();
                }
            }
            
            if (((1 << other.gameObject.layer) & GameBalance.Instance.ItemLayer) != 0)
            {
                var item = other.gameObject.GetComponent<InteractableItemView>();
                if (item != null)
                {
                    item.Model.Destroy(true);
                }
            }
        }
    }
}