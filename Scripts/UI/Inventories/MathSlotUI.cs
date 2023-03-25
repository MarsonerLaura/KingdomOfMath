using RPG.Core.UI.Dragging;
using RPG.Inventories;
using UnityEngine;
using InventoryItem = RPG.Inventories.InventoryItem;

namespace RPG.UI.Inventories
{
    public class MathSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        //TODO maybe display text for numbers instead of icon
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] MathLocation mathLocation = MathLocation.Number;
        [SerializeField] int index = 0;
        
        MathStore mathStore;
        private InventoryItem currentItem;

        #region Basic Unity Methods

        private void OnEnable()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            mathStore = player.GetComponent<MathStore>();
            mathStore.mathStoreUpdated += RedrawUI;
        }
        
        private void Start() 
        {
            RedrawUI();
        }

        #endregion

        #region Main Methods
        
        public int MaxAcceptable(InventoryItem item)
        {
            MathItem mathItem = item as MathItem;
            if (mathItem == null) return 0;
            if (!mathItem.CanEquip(mathLocation)) return 0;
            if (mathStore.IsValueToMuch(currentItem,mathItem,index)) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }

        public bool IsStillValidValueAfterRemove()
        {
            return !(mathStore.IsValueToMuch(currentItem,null,index)) ;
        }
        
        public InventoryItem GetItem()
        {
            return mathStore.GetItemInSlot(currentItem, index);
        }
        
        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        
        public void AddItems(InventoryItem item, int number)
        {
            mathStore.AddItem(currentItem,index,mathLocation, (MathItem)item);
        }
        
        public void RemoveItems(int number)
        {
            mathStore.RemoveItem(currentItem,index);
        }
        
        void RedrawUI()
        {
            currentItem = mathStore.currentItem;
            icon.SetItem(mathStore.GetItemInSlot(currentItem,index));
        }
        
        #endregion

    }
}