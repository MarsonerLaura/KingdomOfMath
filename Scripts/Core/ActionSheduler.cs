using UnityEngine;

namespace RPG.Core
{
    public class ActionSheduler : MonoBehaviour
    {
        private IAction _currentAction;

        #region Main Methods

        public void StartAction(IAction action)
        {
            if (_currentAction == action)
            {
                return;
            }
            if (_currentAction != null)
            {
                _currentAction.Cancel();
            }

            _currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
        
        #endregion
        
    }
}
