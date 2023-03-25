
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("InventorySystem/Math Item"))]
    public class MathItem : InventoryItem
    {

        [Tooltip("Where are we allowed to put this item.")] [SerializeField]
        MathLocation allowedLocation = MathLocation.Number;
        
        [Tooltip("Value of the number, if it is a number.")] [SerializeField]
        private int value;

        [Tooltip("What type of sign it is, if it is a sign.")] [SerializeField]
        private Sign sign;
        
        public enum Sign
        {
            None,
            Add,
            Sub,
            Mul,
            Div
        }

        #region Main Methods

        public bool CanEquip(MathLocation equipLocation)
        {
            if (equipLocation != allowedLocation)
            {
                return false;
            }

            return true;
        }

        #endregion
        
        #region Getters/Setters

        public MathLocation GetAllowedLocation()
        {
            return allowedLocation;
        }

        public int GetValue()
        {
            return value;
        }

        public Sign GetSign()
        {
            return sign;
        }

        #endregion
        
    }
}
