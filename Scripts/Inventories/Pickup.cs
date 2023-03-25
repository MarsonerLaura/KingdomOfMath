using UnityEngine;

namespace RPG.Inventories
{
    public class Pickup : MonoBehaviour
    {
        InventoryItem item;
        int number = 1;
        
        Inventory inventory;

        #region Basic Unity Methods

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
        }

        #endregion
        
        #region Main Methods

        public void Setup(InventoryItem item, int number)
        {
            this.item = item;
            if (!item.IsStackable())
            {
                number = 1;
            }
            this.number = number;
        }

        public void PickupItem()
        {
            bool foundSlot = inventory.AddToFirstEmptySlot(item, number);
            if (foundSlot)
            {
                Destroy(gameObject);
            }
        }

        public bool CanBePickedUp()
        {
            return inventory.HasSpaceFor(item);
        }
        
        #endregion

        #region Getters/Setters

        public InventoryItem GetItem()
        {
            return item;
        }

        public int GetNumber()
        {
            return number;
        }

        #endregion
        
    }
}