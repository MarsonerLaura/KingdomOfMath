using System;
using RPG.Core.UI.Clicking;
using RPG.Core.UI.Dragging;
using RPG.Inventories;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon icon = null;
        int index;
        InventoryItem item;
        Inventory inventory;
        private UIClickHandler _uiClickHandler;

        #region Basic Unity Methods

        private void Awake()
        {
            _uiClickHandler = GetComponent<UIClickHandler>();
            _uiClickHandler.onRightClick += ItemClicked;
        }

        #endregion

        #region Main Methods

        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            inventory.AddItemToSlot(index, item, number);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetNumber()
        {
            return inventory.GetNumberInSlot(index);
        }

        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }
        
        public void ItemClicked()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            MathStore mathStore = player.GetComponent<MathStore>();
            mathStore.currentItem = GetItem();
            if (GetItem() == null || GetNumber() < 1)
            {
                mathStore.currentItem = null;;
            }

            mathStore.ShowItemDetails();
        }

        #endregion
        
    }
}