using RPG.Utils;
using UnityEngine;

namespace RPG.Inventories
{
    public class EquipableItem : InventoryItem
    {
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;
        [SerializeField] private Condition equipCondition;

        #region Main Methods

        public bool CanEquip(EquipLocation equipLocation, Equipment equipment)
        {
            if (equipLocation != allowedEquipLocation)
            {
                return false;
            }

            return equipCondition.Check(equipment.GetComponents<IPredicateEvaluator>());
        }

        #endregion

        #region Getters/Setters

        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }

        #endregion
        
    }
}