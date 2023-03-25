using System;
using RPG.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] [Range(1, 99)] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        [SerializeField] private bool shouldUseModifiers = false;
        
        public event Action onLevelUp;
        
        private LazyValue<int> _currentLevel;
        private Experience _experience = null;

        #region Basic Unity Methods

        private void Awake()
        {
            _experience = GetComponent<Experience>();
            _currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (_experience != null)
            {
                _experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (_experience != null)
            {
                _experience.onExperienceGained -= UpdateLevel;
            }
        }

        #endregion

        #region Main Methods

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)+GetBonusModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }
        
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > _currentLevel.value)
            {
                _currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp?.Invoke();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }
        
        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null)
            {
                return startingLevel;
            }
            float currentXp = experience.GetExperiencePoints();
            int penultimateLevel = progression.GetLevels(Stat.XPLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float xpToLevelUp = progression.GetStat(Stat.XPLevelUp, characterClass, level);
                if (currentXp < xpToLevelUp)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
        
        public float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }
        
        private float GetAdditiveModifier(Stat stat)
        {
            float sum = 0;
            if (shouldUseModifiers)
            {
                IModifierProvider[] providers = GetComponents<IModifierProvider>();
               
                foreach (IModifierProvider provider in providers)
                {
                    foreach (float modifier in provider.GetAdditiveModifiers(stat))
                    {
                        sum += modifier;
                    }
                }
            }
            return sum;
        }
        
        private float GetPercentageModifier(Stat stat)
        {
            float sum = 0;
            if (shouldUseModifiers)
            {
                IModifierProvider[] providers = GetComponents<IModifierProvider>();
                foreach (IModifierProvider provider in providers)
                {
                    foreach (float modifier in provider.GetPercentageModifiers(stat))
                    {
                        sum += modifier;
                    }
                }
            }
            return sum;
        }
        
        private float GetBonusModifier(Stat stat)
        {
            float sum = 0;
            if (shouldUseModifiers)
            {
                IModifierProvider[] providers = GetComponents<IModifierProvider>();
                foreach (IModifierProvider provider in providers)
                {
                    foreach (float modifier in provider.GetBonusModifiers(stat))
                    {
                        sum += modifier;
                    }
                }
            }
            return sum;
        }
        
        #endregion
        
        #region Getters/Setters

        public int GetLevel()
        {
            return _currentLevel.value;
        }

        #endregion
        
    }
}
