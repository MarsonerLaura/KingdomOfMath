using RPG.Attributes;
using RPG.Control;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Dialogue dialogue;
        [SerializeField] private string conversantName;

        #region Main Methods

        public bool HandleRaycast(PlayerController callingController)
        {
            if (dialogue == null)
            {
                //Debug.Log("Dialogue not set.");
                return false;
            }

            Health health = GetComponent<Health>();
            if (health && health.IsDead())
            {
                return false;
            }
            
            if (Mouse.current.leftButton.isPressed)
            {
                callingController.GetComponent<PlayerConversant>().StartDialogueAction(this, dialogue);
            }

            return true;
        }

        #endregion

        #region Getters/Setters

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public string GetConversantName()
        {
            return conversantName;
        }

        public void SetNewDialogue(Dialogue currentActionDialogue)
        {
            dialogue = currentActionDialogue;
        }
        
        #endregion
        
    }
}