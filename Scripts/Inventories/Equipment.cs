using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Utils;
using UnityEngine;


namespace RPG.Inventories
{
    public class Equipment : MonoBehaviour, IJsonSaveable, IPredicateEvaluator
    {
        Dictionary<EquipLocation, EquipableItem> equippedItems = new Dictionary<EquipLocation, EquipableItem>();
        
        public event Action equipmentUpdated;

        #region Main Methods

        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return equippedItems[equipLocation];
        }
        
        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            Debug.Assert(item.CanEquip(slot, this));

            equippedItems[slot] = item;
            equipmentUpdated?.Invoke();
        }
        
        public void RemoveItem(EquipLocation slot)
        {
            equippedItems.Remove(slot);
            equipmentUpdated?.Invoke();
        }
        
        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        }

        #endregion
        
        #region SavingSystem Methods

        public JToken CaptureAsJToken()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.GetItemID();
            }
            return JToken.FromObject(equippedItemsForSerialization);
        }

        public void RestoreFromJToken(JToken state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();
            var equippedItemsForSerialization = state.ToObject<Dictionary<EquipLocation, string>>();
            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    equippedItems[pair.Key] = item;
                }
            }
            equipmentUpdated?.Invoke();
        }

        #endregion

        #region Evaluation Methods

        public bool? Evaluate(IPredicateEvaluator.PredicateType predicate, string[] parameters)
        {
            if (predicate == IPredicateEvaluator.PredicateType.HasItemEquipped)
            {
                foreach (var itemsValue in equippedItems.Values)
                {
                    if (itemsValue.GetItemID() == parameters[0])
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