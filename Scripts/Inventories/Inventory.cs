using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Utils;
using UnityEngine;

namespace RPG.Inventories
{
    
    public class Inventory : MonoBehaviour, IJsonSaveable, IPredicateEvaluator
    {
        [Tooltip("Allowed size")]
        [SerializeField] int inventorySize = 16;
        
        InventorySlot[] slots;

        public struct InventorySlot
        {
            public InventoryItem item;
            public int number;
        }

        #region Basic Unity Methods

        private void Awake()
        {
            slots = new InventorySlot[inventorySize];
        }

        #endregion

        #region Main Methods

        internal void RemoveQuestItems(string itemId, int number)
        {
            InventoryItem itemToRemove = InventoryItem.GetFromID(itemId);
            int slotId = FindStack(itemToRemove);
            RemoveFromSlot(slotId, number);
        }
        
        public event Action inventoryUpdated;
        
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player");
            return player.GetComponent<Inventory>();
        }
        
        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        //returns true if there is enough inventory space for all of the items in the parameter list (stackable taken into account)
        public bool HasSpaceFor(IEnumerable<InventoryItem> items)
        {
            int freeSlots = GetFreeSlots();
            List<InventoryItem> stackedItems = new List<InventoryItem>();
            foreach (var item in items)
            {
                if (item.IsStackable())
                {
                    //is item already in inventory?
                    if (HasItem(item))
                    {
                        continue;
                    }
                    //has item already been seen in the list?
                    if (stackedItems.Contains(item))
                    {
                        continue;
                    }
                    stackedItems.Add(item);
                }
                if (freeSlots <= 0)
                {
                    return false;
                }
                freeSlots--;
            }

            return true;
        }

        
        //returns the number of Free Slots
        public int GetFreeSlots()
        {
            int count = 0;
            foreach (InventorySlot slot in slots)
            {
                if (slot.number == 0)
                {
                    count++;
                }
            }

            return count;
        }
        
        public int GetSize()
        {
            return slots.Length;
        }
        
        public bool AddToFirstEmptySlot(InventoryItem item, int number)
        {
            //add coins to purse
            foreach (var store in GetComponents<IItemStore>())
            {
                number -= store.AddItems(item, number);
            }

            if (number <= 0)
            {
                return true;
            }
            
            //add to slot
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }

            slots[i].item = item;
            slots[i].number += number;
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }
        
        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].item, item))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasItems(InventoryItem item, int number)
        {
            int count = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].item, item))
                {
                    count += slots[i].number;
                }
            }

            if (count >= number)
            {
                return true;
            }
            return false;
        }
        
        public InventoryItem GetItemInSlot(int slot)
        {
            return slots[slot].item;
        }
        
        public int GetNumberInSlot(int slot)
        {
            return slots[slot].number;
        }
        
        public void RemoveFromSlot(int slot, int number)
        {
            slots[slot].number -= number;
            if (slots[slot].number <= 0)
            {
                slots[slot].number = 0;
                slots[slot].item = null;
            }
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }
        
        public bool AddItemToSlot(int slot, InventoryItem item, int number)
        {
            if (slots[slot].item != null)
            {
                return AddToFirstEmptySlot(item, number); ;
            }

            var i = FindStack(item);
            if (i >= 0)
            {
                slot = i;
            }

            slots[slot].item = item;
            slots[slot].number += number;
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }
        
        private int FindSlot(InventoryItem item)
        {
            int i = FindStack(item);
            if (i < 0)
            {
                i = FindEmptySlot();
            }
            return i;
        }
        
        private int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    return i;
                }
            }
            return -1;
        }
        
        private int FindStack(InventoryItem item)
        {
            if (!item.IsStackable())
            {
                return -1;
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].item, item))
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region SavingSystem Methods

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            for (int i = 0; i < inventorySize; i++)
            {
                if (slots[i].item != null)
                {
                    JObject itemState = new JObject();
                    //IDictionary<string, JToken> itemStateDict = itemState;
                    itemState["item"] = JToken.FromObject(slots[i].item.GetItemID());
                    itemState["number"] = JToken.FromObject(slots[i].number);
                    stateDict[i.ToString()] = itemState;
                }
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                slots = new InventorySlot[inventorySize];
                IDictionary<string, JToken> stateDict = stateObject;
                for (int i = 0; i < inventorySize; i++)
                {
                    if (stateDict.ContainsKey(i.ToString()) && stateDict[i.ToString()] is JObject itemState)
                    {
                        IDictionary<string, JToken> itemStateDict = itemState;
                        slots[i].item = InventoryItem.GetFromID(itemStateDict["item"].ToObject<string>());
                        slots[i].number = itemStateDict["number"].ToObject<int>();
                    }
                }
                inventoryUpdated?.Invoke();
            }
        }

        #endregion

        #region Evaluation Methods

        public bool? Evaluate(IPredicateEvaluator.PredicateType predicate, string[] parameters)
        {
            switch (predicate)
            {
             case IPredicateEvaluator.PredicateType.HasInventoryItem:
                 return HasItem(InventoryItem.GetFromID(parameters[0]));
             case IPredicateEvaluator.PredicateType.HasInventoryItems:
                 return HasItems(InventoryItem.GetFromID((parameters[0])), Int32.Parse(parameters[1]));
            }
            return null;
        }

       

        #endregion

    }
}