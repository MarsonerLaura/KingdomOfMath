using System;
using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text damageText = null;

        #region Main Methods

        public void DestroyText()
        {
            Destroy(gameObject);
        }

        public void SetTextValue(float damageAmount)
        {
            damageText.text = String.Format("{0:0}", damageAmount);
        }

        #endregion
        
    }
}
