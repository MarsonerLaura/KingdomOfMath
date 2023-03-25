using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Calculation;
using UnityEngine;
using RPG.Combat;
using RPG.Saving;

namespace RPG.Inventories
{
    public class MathStore : MonoBehaviour, IJsonSaveable
    {
        Dictionary<InventoryItem, Dictionary<int, MathItem>> itemDict = new Dictionary<InventoryItem, Dictionary<int, MathItem>>();
        
        public event Action mathStoreUpdated;
        public InventoryItem currentItem;
        
        private InventoryItem.BonusStat _bonusStat;
        private Calculator _calculator;

        #region Basic Unity Methods

        private void Awake()
        {
            _calculator = GameObject.FindGameObjectWithTag("Player").GetComponent<Calculator>();
        }

        #endregion
        
        #region Main Methods
        
        public bool IsValueToMuch(InventoryItem inventoryItem, MathItem mathItem, int index)
        {  
            float newValue = 0;
            if (!itemDict.ContainsKey(inventoryItem))
            {
                newValue = CalculateNewValue(null, mathItem, index);
                if (newValue <= inventoryItem.GetBonusStat().maxValue && newValue >= 0)
                {
                    return false;
                }
                return true;
            }
            
            Dictionary<int, MathItem> mathItems = itemDict[inventoryItem];
            newValue = CalculateNewValue(mathItems, mathItem, index);
            if (newValue <= inventoryItem.GetBonusStat().maxValue && newValue >= 0)
            {
                return false;
            }

            return true;
        }
        
        public MathItem GetItemInSlot(InventoryItem item, int index)
        {
            if (item == null)
            {
                return null;
            }
            if (itemDict.ContainsKey(item)&&itemDict[item].ContainsKey(index))
            {
                return itemDict[item][index];
            }
            return null;
        }
        
        public void AddItem(InventoryItem item, int index, MathLocation slot, MathItem itemToAdd)
        {
            currentItem = item;
            Debug.Assert(itemToAdd.CanEquip(slot));
            if (!itemDict.ContainsKey(item))
            {
                itemDict[item] = new Dictionary<int, MathItem>();
            }
            itemDict[item][index] = itemToAdd;
            RecalculateBonusStatValue();
            mathStoreUpdated?.Invoke(); 
        }
        
        public void RemoveItem(InventoryItem item, int index)
        {
            itemDict[item].Remove(index);
            RecalculateBonusStatValue();
            mathStoreUpdated?.Invoke(); 
        }
        
        public void ShowItemDetails()
        {
            if (currentItem != null && HasStat())
            {
                RecalculateBonusStatValue();
                mathStoreUpdated?.Invoke();
            }
        }
        
        public bool IsValidItem()
        {
            if (currentItem == null)
            {
                return false;
            }

            if (currentItem is MathItem)
            {
                return false;
            }
            return true;
        }
        
        private void RecalculateBonusStatValue()
        {

                _bonusStat = currentItem.GetBonusStat();

                float newValue = 0;
                if (!itemDict.ContainsKey(currentItem))
                {
                    SetBonusStatValue(newValue);
                    return;
                }
                Dictionary<int, MathItem> mathItems = itemDict[currentItem];
                if (mathItems == null || mathItems.Count == 0)
                {
                    SetBonusStatValue(newValue);
                    return;
                }

                newValue = CalculateNewValue(mathItems);
                SetBonusStatValue(newValue);
            
        }
        
        private float CalculateNewValue(Dictionary<int, MathItem> mathItems, MathItem item = null, int index = -1)
        {
            int[] numbers = new int[4];
            MathItem.Sign[] signs = new MathItem.Sign[3];
            for (int i = 0; i < 7; i++)
            {
                if (mathItems == null || !mathItems.ContainsKey(i))
                {
                    if (i % 2 == 0)
                    {
                        numbers[i / 2] = 0;
                    }
                    else
                    {
                        signs[(i - 1) / 2] = MathItem.Sign.None;
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        numbers[i / 2] = mathItems[i].GetValue();
                    }
                    else
                    {
                        signs[(i - 1) / 2] = mathItems[i].GetSign();
                    }  
                }
                if (i == index)
                {
                    if (i % 2 == 0)
                    {
                        if (item != null)
                        {
                            numbers[i / 2] = item.GetValue();
                        }
                        else
                        {
                            numbers[i / 2] = 0;
                        }
                    }
                    else
                    {
                        if (item != null)
                        {
                            signs[(i - 1) / 2] = item.GetSign();
                        }
                        else
                        {
                            signs[(i - 1) / 2] = MathItem.Sign.None;
                        }
                    }   
                }
            }
            return _calculator.Calculate(numbers, signs);
        }
      
        #endregion

        #region Getters/Setters

        public string GetStatLabel()
        {
            return _bonusStat.bonusStat.ToString();
        }
        
        public float GetMaxValue()
        {
            return _bonusStat.maxValue;
        }

        public string GetDamageValue()
        {
            return ((WeaponConfig) currentItem).GetWeaponDamage().ToString();
        }

        public string GetRangeValue()
        {
            return ((WeaponConfig) currentItem).GetWeaponRange().ToString();
        }

        public bool HasStat()
        {
            return currentItem.HasBonusStat();
        }

        public float GetCurrentValue()
        {
            return _bonusStat.currentValue;
        }
        
        public float GetPercentageValue()
        {
            return (GetCurrentValue()/GetMaxValue())*100;
        }

        private void SetBonusStatValue(float value)
        {
            _bonusStat.currentValue = value;
            currentItem.SetBonusStatValue(value);
        }
        
        #endregion

        #region Saving System Methods

        public JToken CaptureAsJToken()
        {
            var state = new Dictionary<string, Dictionary<int, string>>();
            foreach (var item in itemDict)
            {
                var record = new Dictionary<int, string>();
                foreach (var index in itemDict[item.Key])
                {
                    record[index.Key] = index.Value.GetItemID();
                }
                state[item.Key.GetItemID()] = record;
            }
            return JToken.FromObject(state);
        }

        public void RestoreFromJToken(JToken state)
        {
            itemDict = new Dictionary<InventoryItem, Dictionary<int, MathItem>>();
            var stateDict = state.ToObject<Dictionary<string, Dictionary<int, string>>>();
            foreach (var item in stateDict)
            {
                currentItem = InventoryItem.GetFromID(item.Key);
                foreach (var index in item.Value)
                {
                    if (!itemDict.ContainsKey(currentItem))
                    {
                        itemDict[currentItem] = new Dictionary<int, MathItem>();
                    }
                    itemDict[currentItem][index.Key] = (MathItem)InventoryItem.GetFromID(index.Value);
                    RecalculateBonusStatValue();
                }
               
            }
            currentItem = null;
            mathStoreUpdated?.Invoke();
        }

        #endregion

    }
}
