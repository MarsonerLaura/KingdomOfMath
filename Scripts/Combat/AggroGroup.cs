using System;
using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] private Fighter[] fighters;
        [SerializeField] private bool activateOnStart = false;

        #region Basic Unity Methods

        private void Start()
        {
            Activate(activateOnStart);
        }

        #endregion

        #region Main Methods

        public void Activate(bool shouldActivate)
        {
            foreach (Fighter fighter in fighters)
            {
                CombatTarget target = fighter.GetComponent<CombatTarget>();
                if (target != null)
                {
                    target.enabled = shouldActivate;
                }
                fighter.enabled = shouldActivate;
            }
        }

        #endregion
        
    }
}