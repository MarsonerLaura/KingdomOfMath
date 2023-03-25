using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        #region Basic Unity Methods

        private void LateUpdate()
        {
            transform.position = _target.position;
        }

        #endregion
        
    }
}
