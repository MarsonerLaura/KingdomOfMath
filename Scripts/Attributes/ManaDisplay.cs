using System;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        private Mana _mana = null;
        private TMP_Text _text = null;

        #region Basic Unity Methods

        private void Awake()
        {
            _mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            _text.text = String.Format("{0:0}/{1:0}", _mana.GetMana(),_mana.GetMaxMana());
        }

        #endregion
        
    }
}