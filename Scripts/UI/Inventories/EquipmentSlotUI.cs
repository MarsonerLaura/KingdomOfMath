
using RPG.Core.UI.Clicking;
using RPG.Core.UI.Dragging;
using RPG.Inventories;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] EquipLocation equipLocation = EquipLocation.Weapon;
        
        Equipment playerEquipment;
        private UIClickHandler _uiClickHandler;

        #region Basic Unity Methods

        private void Awake() 
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            playerEquipment = player.GetComponent<Equipment>();
            playerEquipment.equipmentUpdated += RedrawUI;
            _uiClickHandler = GetComponent<UIClickHandler>();
            _uiClickHandler.onRightClick += ItemClicked;
        }

        private void Start() 
        {
            RedrawUI();
        }

        #endregion

        #region Main Methods

        public int MaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;
            if (equipableItem == null) return 0;
            if (!equipableItem.CanEquip(equipLocation, playerEquipment)) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }

        public void AddItems(InventoryItem item, int number)
        {
            playerEquipment.AddItem(equipLocation, (EquipableItem) item);
        }

        public InventoryItem GetItem()
        {
            return playerEquipment.GetItemInSlot(equipLocation);
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

        public void RemoveItems(int number)
        {
            playerEquipment.RemoveItem(equipLocation);
        }

        void RedrawUI()
        {
            icon.SetItem(playerEquipment.GetItemInSlot(equipLocation));
        }
        
        public void ItemClicked()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            MathStore mathStore = player.GetComponent<MathStore>();
            mathStore.currentItem = GetItem();
            if (GetItem() == null || GetNumber() < 1)
            {
                mathStore.currentItem = null;;
            }
            
            mathStore.ShowItemDetails();
        }
        
        #endregion
        
    }
}