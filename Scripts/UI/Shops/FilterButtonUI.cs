using System;
using RPG.Inventories;
using RPG.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        [SerializeField] private ItemCategory category = ItemCategory.None;
        
        private Button _button;
        private Shop _currentShop;
        
        #region Basic Unity Methods
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(SelectFilter);
        }

        #endregion

        #region Main Methods
        
        public void SetShop(Shop currentShop)
        {
            _currentShop = currentShop;
        }

        //changes the filter of the shop (category) on click
        private void SelectFilter()
        {
            _currentShop.SelectFilter(category);
        }

        //disables button if it is current filter/ enables if not
        public void RefreshUI()
        {
            _button.interactable = _currentShop.GetFilter() != category;
        }

        #endregion
        
    }
}