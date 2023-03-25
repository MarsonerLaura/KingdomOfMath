using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private string action;
        [SerializeField] private UnityEvent onTrigger;

        #region Main Methods

        public void Trigger(string actionToTrigger)
        {
            if (actionToTrigger.Equals(action))
            {
                onTrigger?.Invoke();
            }
        }

        #endregion
        
    }
}

