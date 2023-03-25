using RPG.Inventories;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        private Pickup _pickup;

        #region Basic Unity Methods

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
        }

        #endregion

        #region Main Methods

        public CursorType GetCursorType()
        {
            if (_pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.FullPickup;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                _pickup.PickupItem();
            }
            return true;
        }

        #endregion
        
    }
}