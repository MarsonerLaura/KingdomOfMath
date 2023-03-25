using RPG.Inventories;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopItem
    {
        private InventoryItem _item;
        private int _availability;
        private float _price;
        private int _quantityInTransaction;

        public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction)
        {
            _item = item;
            _availability = availability;
            _price = price;
            _quantityInTransaction = quantityInTransaction;
        }

        #region Getters and Setters

        public string GetName()
        {
            return _item.GetDisplayName();
        }


        public int GetAvailability()
        {
            return _availability;
        }

        public float GetPrice()
        {
            return _price;
        }

        public int GetQuantityInTransaction()
        {
            return _quantityInTransaction;
        }

        public Sprite GetIcon()
        {
            return _item.GetIcon();
        }
        
        public InventoryItem GetInventoryItem()
        {
            return _item;
        }

        #endregion
        
    }
}


