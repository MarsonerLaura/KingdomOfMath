using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private TextMeshProUGUI availability;
        [SerializeField] private TextMeshProUGUI price;
        [SerializeField] private TextMeshProUGUI quantityInTransaction;

        private Shop _shop = null;
        private ShopItem _item = null;
        
        #region Main Methods

        public void Add()
        {
            _shop.AddToTransaction(_item.GetInventoryItem(), 1);
        }

        public void Remove()
        {
            _shop.AddToTransaction(_item.GetInventoryItem(), -1);
        }
        
        public void Setup(Shop currentShop, ShopItem shopItem)
        {
            _shop = currentShop;
            _item = shopItem;
            icon.sprite = shopItem.GetIcon();
            name.text = shopItem.GetName();
            availability.text = $"{shopItem.GetAvailability()}";
            price.text = $"{shopItem.GetPrice():N2} ";
            quantityInTransaction.text =  $"{shopItem.GetQuantityInTransaction()}";
        }
        
        #endregion
        
    }
}
