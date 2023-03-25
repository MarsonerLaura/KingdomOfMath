using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHit;

        #region Main Methods

        public void OnHit()
        {
            onHit.Invoke();
        }

        #endregion
        
    }
}
