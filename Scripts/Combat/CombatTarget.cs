using RPG.Attributes;
using RPG.Control;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        #region Main Methods

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!enabled)
            {
                return false;
            }
            if (!callingController.GetFighter().CanAttack(gameObject ))
            {
                return false;
            }
            if (Mouse.current.leftButton.isPressed)
            {
                callingController.GetFighter().Attack(gameObject);
            }
            return true;
        }

        #endregion
        
    }
}
