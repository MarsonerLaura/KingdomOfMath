using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience _experience;
        private TMP_Text _text;

        #region Basic Unity Methods
        
        private void Awake()
        {
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            _text.text = String.Format("{0:0}", _experience.GetExperiencePoints());
        }
        
        #endregion
        
    }
}