
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("InventorySystem/Action Item"))]
    public class ActionItem : InventoryItem
    {
        [Tooltip("Does an instance of this item get consumed every time it's used.")]
        [SerializeField] bool consumable = false;
        
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] ActionLocation allowedEquipLocation = ActionLocation.Consumable;


        #region Getters/Setters

        public ActionLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }
        
        public bool isConsumable()
        {
            return consumable;
        }

        #endregion

        #region Main Methods

        //default Action Item, does nothing
        public virtual bool Use(GameObject user)
        {
            Debug.Log("Using action: " + this);
            return false;
            
        }

        #endregion
       
    }
}