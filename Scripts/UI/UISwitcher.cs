using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.UI
{
    public class UISwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject entryPoint;

        #region Basic Unity Methods

        private void Start()
        {
            SwitchTo(entryPoint);
        }

        #endregion

        #region Main Methods

        public void SwitchTo(GameObject toDisplay)
        {
            if (toDisplay.transform.parent != transform)
            {
                return;
            }
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(child.gameObject.Equals(toDisplay));
            }
        }

        #endregion
        
    }
}
