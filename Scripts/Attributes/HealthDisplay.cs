using System;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health _health = null;
        private TMP_Text _text = null;

        #region Basic Unity Methods

        private void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            _text.text = String.Format("{0:0}/{1:0}", _health.GetHealthPoints(),_health.GetMaxHealthPoints());
        }

        #endregion
        
    }
}
