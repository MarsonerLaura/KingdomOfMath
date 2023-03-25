using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Combat;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class PlayerInfoUI : MonoBehaviour
    {

        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private Slider _manaSlider;
        [SerializeField] private TMP_Text _manaText;
        [SerializeField] private Slider _experienceSlider;
        [SerializeField] private TMP_Text _experienceText;
        [SerializeField] private TMP_Text _levelText;

        private Health _health;
        private Mana _mana;
        private Experience _experience;
        private BaseStats _baseStats;

        #region Basic Unity Methods

        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _health = player.GetComponent<Health>();
            _mana = player.GetComponent<Mana>();
            _experience = player.GetComponent<Experience>();
            _baseStats = player.GetComponent<BaseStats>();
        }
        
        private void Update()
        {
            _healthSlider.value = _health.GetHealthPoints() / _health.GetMaxHealthPoints();
            _healthText.text = String.Format("{0:0}/{1:0}", _health.GetHealthPoints(),_health.GetMaxHealthPoints());
            _manaSlider.value = _mana.GetMana() / _mana.GetMaxMana();
            _manaText.text = String.Format("{0:0}/{1:0}", _mana.GetMana(),_mana.GetMaxMana());
            _experienceSlider.value = _experience.GetExperiencePoints() / _baseStats.GetBaseStat(Stat.XPLevelUp);
            _experienceText.text = String.Format("{0:0}/{1:0}", _experience.GetExperiencePoints(), _baseStats.GetBaseStat(Stat.XPLevelUp));
            _levelText.text = String.Format("{0:0}", _baseStats.GetLevel());
        }

        #endregion
        
    }
}
