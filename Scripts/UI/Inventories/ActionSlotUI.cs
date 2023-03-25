 using RPG.Abilities;
 using RPG.Core.UI.Clicking;
 using RPG.Core.UI.Dragging;
using RPG.Inventories;
using UnityEngine;using UnityEngine.UI;
 using InventoryItem = RPG.Inventories.InventoryItem;

 namespace RPG.UI.Inventories
{
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] int index = 0;
        [SerializeField] ActionLocation equipLocation = ActionLocation.Consumable;
        [SerializeField] private Image cooldownOverlay = null;
        
        ActionStore store;
        private CooldownStore _cooldownStore;
        private UIClickHandler _uiClickHandler;

        #region Basic Unity Methods

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            _cooldownStore = player.GetComponent<CooldownStore>();
            store = player.GetComponent<ActionStore>();
            store.storeUpdated += UpdateIcon;
            _uiClickHandler = GetComponent<UIClickHandler>();
            _uiClickHandler.onRightClick += ItemClicked;
        }

        private void Update()
        {
            cooldownOverlay.fillAmount =_cooldownStore.GetCooldownFractionRemaining(GetItem());
        }

        #endregion

        #region Main Methods

        public void AddItems(InventoryItem item, int number)
        {
            store.AddAction(item, index, number);
        }

        public InventoryItem GetItem()
        {
            return store.GetAction(index);
        }

        public int GetNumber()
        {
            return store.GetNumber(index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            ActionItem equipableItem = item as ActionItem;
            if (equipableItem == null)
            {
                return 0;
            }

            if (equipableItem.GetAllowedEquipLocation() != equipLocation)
            {
                return 0;
            }
            return store.MaxAcceptable(item, index);
        }

        public void RemoveItems(int number)
        {
            store.RemoveItems(index, number);
        }

        void UpdateIcon()
        {
            icon.SetItem(GetItem(), GetNumber());
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