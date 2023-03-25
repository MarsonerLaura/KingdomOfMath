using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Abilities;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;

namespace RPG.Inventories
{
    public class ActionStore : MonoBehaviour, IJsonSaveable, IModifierProvider, IPredicateEvaluator
    {
        Dictionary<int, DockedItemSlot> dockedItems = new Dictionary<int, DockedItemSlot>();
        private class DockedItemSlot 
        {
            public ActionItem item;
            public int number;
        }
        
        public event Action storeUpdated;
        
        [System.Serializable]
        private struct DockedItemRecord
        {
            public string itemID;
            public int number;
        }

        #region Main Methods

        public ActionItem GetAction(int index)
        {
            if (dockedItems.ContainsKey(index))
            {
                return dockedItems[index].item;
            }
            return null;
        }
        
        public int GetNumber(int index)
        {
            if (dockedItems.ContainsKey(index))
            {
                return dockedItems[index].number;
            }
            return 0;
        }
        
        public void AddAction(InventoryItem item, int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {  
                if (object.ReferenceEquals(item, dockedItems[index].item))
                {
                    dockedItems[index].number += number;
                }
            }
            else
            {
                var slot = new DockedItemSlot();
                slot.item = item as ActionItem;
                slot.number = number;
                dockedItems[index] = slot;
            }
            if (storeUpdated != null)
            {
                storeUpdated();
            }
        }
        
        public bool Use(int index, GameObject user)
        {
            if (dockedItems.ContainsKey(index))
            {
                bool wasUsed = dockedItems[index].item.Use(user);
                if (wasUsed && dockedItems[index].item.isConsumable())
                {
                    RemoveItems(index, 1);
                }
                return true;
            }
            return false;
        }
        
        public void RemoveItems(int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {
                dockedItems[index].number -= number;
                if (dockedItems[index].number <= 0)
                {
                    dockedItems.Remove(index);
                }
                storeUpdated?.Invoke();
            }
            
        }
        
        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;
            if (!actionItem) return 0;

            if (dockedItems.ContainsKey(index) && !object.ReferenceEquals(item, dockedItems[index].item))
            {
                return 0;
            }
            if (actionItem.isConsumable())
            {
                return int.MaxValue;
            }
            if (dockedItems.ContainsKey(index))
            {
                return 0;
            }

            return 1;
        }
        
        public IEnumerable<int> GetAllPopulatedSlots()
        {
            return dockedItems.Keys;
        }
        
        public ActionItem GetItemInSlot(int index)
        {
            if (!dockedItems.ContainsKey(index))
            {
                return null;
            }

            return dockedItems[index].item;
        }

        #endregion

        #region SavingSystem Methods

        public JToken CaptureAsJToken()
        {
            var state = new Dictionary<int, DockedItemRecord>();
            foreach (var pair in dockedItems)
            {
                var record = new DockedItemRecord();
                record.itemID = pair.Value.item.GetItemID();
                record.number = pair.Value.number;
                state[pair.Key] = record;
            }

            return JToken.FromObject(state);
        }

        public void RestoreFromJToken(JToken state)
        {
            dockedItems = new Dictionary<int, DockedItemSlot>();
            var stateDict = state.ToObject<Dictionary<int, DockedItemRecord>>();
            foreach (var pair in stateDict)
            {
                AddAction(InventoryItem.GetFromID(pair.Value.itemID), pair.Key, pair.Value.number);
            }
            storeUpdated?.Invoke();
        }

        #endregion
        
        #region Modifier Methods
        
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        { 
            yield break;
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            yield break;
        }
        
        public IEnumerable<float> GetBonusModifiers(Stat stat)
        {
            foreach (var slotIndex in GetAllPopulatedSlots())
            {
                var abilityItem = GetItemInSlot(slotIndex) as Ability;
      
                var item = abilityItem as IModifierProvider;
                if (item == null)
                {
                    continue;
                }
                foreach (float modifier in item.GetBonusModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
        
        #endregion

        #region Evaluation Methods

        public bool? Evaluate(IPredicateEvaluator.PredicateType predicate, string[] parameters)
        {
            if (predicate == IPredicateEvaluator.PredicateType.HasActionItemEquipped)
            {
                foreach (var itemsValue in dockedItems.Values)
                {
                    if (itemsValue.item.GetItemID() == parameters[0])
                    {
                        return true;
                    }
                }

                return false;
            }
            return null;
        }

        #endregion
        
    }
}