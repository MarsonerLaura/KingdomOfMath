using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI shopNameField;
        [SerializeField] private Transform listRoot;
        [SerializeField] private RowUI rowPrefab;
        [SerializeField] private TextMeshProUGUI totalField;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button switchModeButton;

        private Shopper _shopper = null;
        private Shop _currentShop = null;
        private Color originalTotalTextColor;

        #region Basic Unity Methods

        private void Awake()
        {
            _shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
            originalTotalTextColor = totalField.color;
        }

        private void Start()
        {
            if (_shopper == null)
            {
                return;
            }
            _shopper.ActiveShopChanged += ShopChanged;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchModeButton.onClick.AddListener(SwitchMode);
            ShopChanged();
        }
        
        #endregion

        #region Main Methods

        //closes the shop ui
        public void Close()
        {
            _shopper.SetActiveShop(null);
        }

        //called by buy/sell button 
        public void ConfirmTransaction()
        {
            _currentShop.ConfirmTransaction();
        }

        //switches the mode (selling or buying)
        public void SwitchMode()
        {
            _currentShop.SelectMode(!_currentShop.IsBuyingMode());
        }
        
        //is called when THE shop is changed, updates ui
        private void ShopChanged()
        {
            if (_currentShop != null)
            {
                _currentShop.OnChange -= RefreshUI;
            }
            _currentShop = _shopper.GetActiveShop();
            gameObject.SetActive(_currentShop!=null);
            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetShop(_currentShop);
            }
            if (_currentShop == null)
            {
                return;
            }
            shopNameField.text = _currentShop.GetShopName();
            _currentShop.OnChange += RefreshUI;
            RefreshUI();
        }

        //called when something IN the shop is updated, updates ui
        private void RefreshUI()
        {
            foreach (Transform row in listRoot)
            {
                Destroy(row.gameObject);
            }

            foreach (ShopItem shopItem in _currentShop.GetFilteredItems())
            {
                 RowUI rowInstance = Instantiate<RowUI>(rowPrefab, listRoot);
                 rowInstance.Setup(_currentShop, shopItem);
            }

            totalField.text = $"Gesamt: {_currentShop.GetTransactionTotal():N2} ";
            totalField.color = _currentShop.HasSufficientMoney() ? originalTotalTextColor : Color.red;
            confirmButton.interactable = _currentShop.CanTransact();
            
            TextMeshProUGUI switchText = switchModeButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if (_currentShop.IsBuyingMode())
            {
                switchText.text = "Wechsel zu Verkaufen";
                confirmText.text = "Kaufen";
            }
            else
            {
                switchText.text = "Wechsel zu Kaufen";
                confirmText.text = "Verkaufen";
            }
            
            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.RefreshUI();
            }
        }

        #endregion

    }
}
