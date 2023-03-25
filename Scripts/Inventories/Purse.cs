using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, IJsonSaveable, IItemStore
    {
        [SerializeField] private float startingBalance = 420f;

        public event Action OnChange;
        
        private float _balance = 0;

        #region Basic Unity Methods

        private void Awake()
        {
            _balance = startingBalance;
        }

        #endregion

        #region Getters and Setters

        public float GetBalance()
        {
            return _balance;
        }
        
        #endregion

        #region Main Methods

        public void UpdateBalance(float amount)
        {
            _balance += amount;
            OnChange?.Invoke();
        }
        
        public int AddItems(InventoryItem item, int number)
        {
            if (item is CurrencyItem)
            {
                UpdateBalance(item.GetPrice() * number);
                return number;
            }
            return 0;
        }

        #endregion

        #region Saving System Methods

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_balance);
        }

        public void RestoreFromJToken(JToken state)
        {
            _balance = state.ToObject<float>();
            OnChange?.Invoke();
        }
        
        #endregion
        
    }
}
