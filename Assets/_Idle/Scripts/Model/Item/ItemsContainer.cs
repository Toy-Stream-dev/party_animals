using System.Collections.Generic;
using System.Linq;
using _Idle.Scripts.Model.Base;
using _Idle.Scripts.Model.Item;
using _Idle.Scripts.View.Item;
using UnityEngine;

namespace _Idle.Scripts.Model.Unit
{
    public class ItemsContainer : ModelsContainer<InteractableItemModel>
    {
        public InteractableItemModel SpawnItem()
        {
            var item = Create()
                .SpawnView(MainGame.ItemsContainer)
                .Start();

            return item;
        }

        public InteractableItemModel CreateItem(InteractableItemView view)
        {
            var item = Create();
            item.SetView(view);
            item.Start();

            return item;
        }

        public void Clear()
        {
            for (int i = 0, count = All.Count; i < count; i++)
            {
                All[i].DestroyView();
                All[i].End();
            }

            All.Clear();
        }

        public void FindExistingItems(Transform parent)
        {
            var items = parent.GetComponentsInChildren<InteractableItemView>();
            for (var i = 0; i < items.Length; i++)
            {
                var model = Create();
                model.SetView(items[i]);
                model.Start();
            }
        }

        public void DestroyAll(bool onlyWeapon = false)
        {
            if (onlyWeapon)
            {
                var weapons = All.Where(item => item.Params.IsWeapon).ToList();
                foreach (var item in weapons)
                {
                    All.Remove(item);
                    item.Destroy();
                }
            }
            else
            {
                foreach (var item in All)
                {
                    item.Destroy();
                }
                All.Clear();
            }
        }

        public void Remove(InteractableItemModel item)
        {
            All.Remove(item);
        }

        public int GetItemsCount()
        {
            return All.Count;
        }

        public List<InteractableItemModel> GetAll()
        {
            return All;
        }
    }
}