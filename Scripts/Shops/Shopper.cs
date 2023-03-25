using System;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        private Shop _activeShop = null;
        public event Action ActiveShopChanged;

        #region Getters and Setters

        public Shop GetActiveShop()
        {
            return _activeShop;
        }
        
        public void SetActiveShop(Shop shop)
        {
            if (_activeShop != null)
            {
                _activeShop.SetShopper(null);
            }
            _activeShop = shop;
            if (_activeShop != null)
            {
                _activeShop.SetShopper(this);
            }
            ActiveShopChanged?.Invoke();
        }

        #endregion
        
    }

}