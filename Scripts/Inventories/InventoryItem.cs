using System;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{

    [CreateAssetMenu(menuName = ("InventorySystem/Basic Item"))]
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [SerializeField] string itemID = null;
        [Tooltip("Item name to be displayed in UI.")]
        [SerializeField] string displayName = null;
        [Tooltip("Item description to be displayed in UI.")]
        [SerializeField][TextArea] string description = null;
        [Tooltip("The UI icon to represent this item in the inventory.")]
        [SerializeField] Sprite icon = null;
        [Tooltip("The prefab that should be spawned when this item is dropped.")]
        [SerializeField] Pickup pickup = null;
        [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
        [SerializeField] bool stackable = false;
        [Tooltip("The price this item will be sold in the shops")]
        [SerializeField] private float price = 0f;
        [Tooltip("The category this item belongs to and can be filtered in shops")] 
        [SerializeField] private ItemCategory category = ItemCategory.None;
        [Tooltip("The level of this item.")] 
        [SerializeField] private int level = 1;
        [Tooltip("True if item has BonusStat.")] 
        [SerializeField] private bool hasBonusStat;
        [Tooltip("The stat where Math can be used to enhance its value.")] 
        [SerializeField] private BonusStat bonusStat;
    
        static Dictionary<string, InventoryItem> itemLookupCache;

        
        [System.Serializable]
        public struct BonusStat
        {
            public Stat bonusStat;
            public float maxValue;
            public float currentValue; 
            
        }

        #region Main Methods

        public static InventoryItem GetFromID(string itemID)
        {
            if (itemLookupCache == null)
            {
                itemLookupCache = new Dictionary<string, InventoryItem>();
                var itemList = Resources.LoadAll<InventoryItem>("");
                foreach (var item in itemList)
                {
                    if (itemLookupCache.ContainsKey(item.itemID))
                    {
                        Debug.LogError(string.Format("Looks like there's a duplicate GameDevTV.UI.InventorySystem ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                        continue;
                    }

                    itemLookupCache[item.itemID] = item;
                }
            }

            if (itemID == null || !itemLookupCache.ContainsKey(itemID)) return null;
            return itemLookupCache[itemID];
        }
        
        public Pickup SpawnPickup(Vector3 position, int number)
        {
            var pickup = Instantiate(this.pickup);
            pickup.transform.position = position;
            pickup.Setup(this, number);
            return pickup;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(itemID))
            {
                itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver 
        }
        
        #endregion

        #region Getters/Setters

        public float GetPrice()
        {
            return price;
        }

        public ItemCategory GetCategory()
        {
            return category;
        }

        public int GetLevel()
        {
            return level;
        }

        public bool HasBonusStat()
        {
            return hasBonusStat;
        }

        public BonusStat GetBonusStat()
        {
            return bonusStat;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public string GetItemID()
        {
            return itemID;
        }

        public bool IsStackable()
        {
            return stackable;
        }
        
        public string GetDisplayName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return description;
        }
        
        public void SetBonusStatValue(float value)
        {
            bonusStat.currentValue = value;
        }
        
        #endregion
        
    }
}
