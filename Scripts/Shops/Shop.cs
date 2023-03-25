using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Newtonsoft.Json.Linq;
using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

using RPG.Movement;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, IJsonSaveable
    {
        [SerializeField] private string shopName;
        [SerializeField] private StockItemConfig[] stockConfig;
        [Tooltip("how much less does the player get when selling items")]
        [SerializeField][Range(-100,100)] private float shopSellingPercentage = 30f;
        [SerializeField] private float maximumNiceGuyDiscount = 80f;

        public event Action OnChange;

        private Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();
        private Dictionary<InventoryItem, int> _stockSold = new Dictionary<InventoryItem, int>();
        private Shopper _currentShopper = null;
        private bool _isBuyingMode = true;
        private ItemCategory _filter = ItemCategory.None;
        private PlayerController _targetController;

        [System.Serializable]
        class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(-100,100)] public float buyingDiscountPercentage;
            [Range(-100,100)] public float sellingPercentage = 80f;
            public int levelToUnlock = 0;
        }

        #region Main Methods

        //returns all items in the shop
        public IEnumerable<ShopItem> GetAllItems()
        {
            Dictionary<InventoryItem, float> prices = GetPrices();
            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();
            foreach (InventoryItem item in availabilities.Keys)
            {
                if (availabilities[item] <= 0)
                {
                    continue;
                }
                float price = prices[item];
                int quantityInTransaction = 0;
                _transaction.TryGetValue(item, out quantityInTransaction);
                int availability = availabilities[item];
                yield return new ShopItem(item, availability, price,quantityInTransaction);
            }
        }

        //returns the items filtered by the set category
        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (ShopItem item in GetAllItems())
            {
                if (_filter == ItemCategory.None || _filter == item.GetInventoryItem().GetCategory())
                {
                    yield return item;
                }

            }
        }

        //buy or sell items
        public void ConfirmTransaction()
        {
            Inventory shopperInventory = _currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = _currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null)
            {
                return;
            }

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                float price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (_isBuyingMode)
                    {
                        BuyItem(shopperPurse, price, shopperInventory, item);
                    }
                    else
                    {
                        SellItem(shopperPurse, price, shopperInventory, item);
                    }
                }
            }
            OnChange?.Invoke();
        }
        
        //select a category to filter the items for
        public void SelectFilter(ItemCategory category)
        {
            _filter = category;
            OnChange?.Invoke();
        }

        //switch between buying and sellingmode
        public void SelectMode(bool isBuying)
        {
            _transaction.Clear();
            _isBuyingMode = isBuying;
            OnChange?.Invoke();
        }
        
        //adds or removes a quantity of an item to/from the transaction and checks if there is enough stock available
        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!_transaction.ContainsKey(item))
            {
                _transaction[item] = 0;
            }

            var availabilities = GetAvailabilities();
            int availability = availabilities[item];
            
            //checks if there are enough items available
            if (_transaction[item] + quantity > availability)
            {
                _transaction[item] = availability;
            }
            else
            {
                _transaction[item] += quantity;
            }
            
            if (_transaction[item] <= 0)
            {
                _transaction.Remove(item);
            }
            OnChange?.Invoke();
        }
        
        //returns the sum of all prices in the transaction
        public float GetTransactionTotal()
        {
            float total = 0;
            foreach (ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }

            return total;
        }
        
        //returns true if a transaction is possible (not empty, enough money, enough inventory space)
        public bool CanTransact()
        {
            if (IsTransactionEmpty())
            {
                return false;
            }

            if (!HasSufficientMoney())
            {
                return false;
            }

            if (!HasEnoughInventorySpace())
            {
                return false;
            }
            return true;
        }
      
        public bool IsTransactionEmpty()
        {
            return _transaction.Count == 0;
        }

        //returns true if the player has enough money to buy all items in transaction
        public bool HasSufficientMoney()
        {
            if (!_isBuyingMode)
            {
                return true;
            }
            Purse purse = _currentShopper.GetComponent<Purse>();
            if (purse == null)
            {
                return false;
            }
            if (purse.GetBalance() < GetTransactionTotal())
            {
                return false;
            }
            return true;
        }
        
        //returns true if the player has enough inventory space to add all items from the transaction
        public bool HasEnoughInventorySpace()
        {
            if (!_isBuyingMode)
            {
                return true;
            }
            Inventory currentInventory = _currentShopper.GetComponent<Inventory>();
            if (currentInventory == null)
            {
                return false;
            }
            
            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }
            
            return currentInventory.HasSpaceFor(flatItems);
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = _currentShopper.GetComponent<Inventory>();
            if (inventory == null)
            {
                return 0;
            }
            
            int count = 0;
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (item.Equals(inventory.GetItemInSlot(i)))
                {
                    count += inventory.GetNumberInSlot(i);
                } 
            }

            return count;
        }

        //transaction for buying an item (removing it from shop, aading to inventory, paying)
        private void BuyItem(Purse shopperPurse, float price, Inventory shopperInventory, InventoryItem item)
        {
            if (shopperPurse.GetBalance() < price)
            {
                return;
            }

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                if (!_stockSold.ContainsKey(item))
                {
                    _stockSold[item] = 0;
                }
                _stockSold[item]++;
                shopperPurse.UpdateBalance(-price);
            }
        }
        
        //transaction for selling an item (removing from inventory, adding to shop, getting money)
        private void SellItem(Purse shopperPurse, float price, Inventory shopperInventory, InventoryItem item)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1)
            {
                return;
            }
            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot,1);
            if (!_stockSold.ContainsKey(item))
            {
                _stockSold[item] = 0;
            }
            _stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }

        //returns the id of the Inventory slot where the item is first found (-1 if not in inventory)
        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }
            return -1;
        }
        
        //returns the availabilities of the stockitems per item
        private Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();
            foreach (var config in GetAvailableConfigs())
            {
                if (_isBuyingMode)
                {
                    if (!availabilities.ContainsKey(config.item))
                    {
                        int sold = 0;
                        _stockSold.TryGetValue(config.item, out sold);
                        availabilities[config.item] = -sold;
                    }

                    availabilities[config.item] += config.initialStock;
                }
                else
                {
                    availabilities[config.item] = CountItemsInInventory(config.item);
                }
            }

            return availabilities;
        }
        
        //returns the prices of the stockitems per item
        private Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();
            foreach (var config in GetAvailableConfigs())
            {
                if (_isBuyingMode)
                {
                    if (!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = config.item.GetPrice() * GetNiceGuyDiscount();
                    }
                    prices[config.item] *= (1 - config.buyingDiscountPercentage / 100);
                }
                else if (config.sellingPercentage == 0)
                {
                    prices[config.item]= config.item.GetPrice() * (shopSellingPercentage / 100);
                }
                else
                {
                    prices[config.item]= config.item.GetPrice() * (config.sellingPercentage / 100);
                }
            }
            return prices;
        }

        //returns the discount for charisma
        private float GetNiceGuyDiscount()
        {
            BaseStats baseStats =  _currentShopper.GetComponent<BaseStats>();
            float discount = baseStats.GetStat(Stat.KaufRabatt);
            return (1 - Mathf.Min(discount, maximumNiceGuyDiscount) / 100);
        }

        //returns all items that are available considering the players level
        private IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach (var config in stockConfig)
            {
                if (config.levelToUnlock > shopperLevel)
                {
                    continue;
                }
                yield return config;
            }
        }

        #endregion
        
        #region Getters and Setters

        public void SetShopper(Shopper shopper)
        {
            _currentShopper = shopper;
        }
        
        public ItemCategory GetFilter()
        {
            return _filter;
        }

        public bool IsBuyingMode()
        {
            return _isBuyingMode;
        }

        public string GetShopName()
        {
            return shopName;
        }
        
        private int GetShopperLevel()
        {
            BaseStats stats = _currentShopper.GetComponent<BaseStats>();
            if (stats == null)
            {
                return 0;
            }

            return stats.GetLevel();
        }
        
        #endregion

        #region IRaycastable Methods

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                _targetController = callingController;
                if (Vector3.Distance(_targetController.transform.position, this.transform.position) > 5)
                {
                    _targetController.GetComponent<Mover>().StartMoveAction(this.transform.position, 1);
                }
                else
                {
                    _targetController.GetComponent<Mover>().Cancel();
                    _targetController.GetComponent<Shopper>().SetActiveShop(this);
                    _targetController = null;
                }
                
            }
            return true;
        }
        
        private void Update()
        {
            if (_targetController && Vector3.Distance(_targetController.transform.position, this.transform.position) < 5)
            {
                    _targetController.GetComponent<Mover>().Cancel();
                    _targetController.GetComponent<Shopper>().SetActiveShop(this);
                    _targetController = null;
                
            }
        }
        
        #endregion

        #region Saving System Methods

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            foreach (KeyValuePair<InventoryItem,int> pair in _stockSold)
            {
                stateDict[pair.Key.GetItemID()] = JToken.FromObject(pair.Value);
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                IDictionary<string, JToken> stateDict = stateObject;
                _stockSold.Clear();
                foreach (KeyValuePair<string,JToken> pair in stateDict)
                {
                    InventoryItem item = InventoryItem.GetFromID(pair.Key);
                    if (item)
                    {
                        _stockSold[item] = pair.Value.ToObject<int>();
                    }
                }
            }
            OnChange?.Invoke();
        }
        
        #endregion
        
    }
}
