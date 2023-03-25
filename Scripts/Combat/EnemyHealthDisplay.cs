using System;
using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter _fighter;
        private TMP_Text _text;

        #region Basic Unity Methods

        private void Awake()
        {
            _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            _text = GetComponent<TMP_Text>();
        }
        
        private void Update()
        {
            if (_fighter.GetTarget() == null)
            {
                _text.text = "N/A";
            }
            else
            {
                Health health = _fighter.GetTarget();
                _text.text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(),health.GetMaxHealthPoints());
            }
        }

        #endregion
        
    }
}
