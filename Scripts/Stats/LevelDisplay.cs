using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats _baseStats;
        private TMP_Text _text;

        #region Basic Unity Methods

        private void Awake()
        {
            _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            _text = GetComponent<TMP_Text>();
        }
        
        private void Update()
        {
            _text.text = String.Format("{0:0}", _baseStats.GetLevel());
        }

        #endregion
        
    }
}
