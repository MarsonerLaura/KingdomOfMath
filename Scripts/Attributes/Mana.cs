using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float regenerationPercentage = 70f;
        
        private BaseStats _baseStats;
        private LazyValue<float> _mana;
        bool inCombat;

        #region Basic Unity Methods
        
        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();
            _mana = new LazyValue<float>(GetMaxMana);
        }
        private void Update()
        {
            if (_mana.value < GetMaxMana())
            {
                //test if in combat 
                //_mana.value += GetRegenRateOutOfCombat() * Time.deltaTime;
                if (!inCombat)
                {
                    _mana.value += GetRegenRateOutOfCombat() * Time.deltaTime;
                }
                else
                {
                    _mana.value += GetRegenRate() * Time.deltaTime;
                }
                if (_mana.value > GetMaxMana())
                {
                    _mana.value = GetMaxMana();
                }
            }
        }

        private void Start()
        {
            _mana.ForceInit();
        }
        
        private void OnEnable()
        {
            _baseStats.onLevelUp += RegenerateMana;
        }

        private void OnDisable()
        {
            _baseStats.onLevelUp -= RegenerateMana;
        }
        #endregion
        
        #region Getters and Setters 

        
        public void SetInCombat(bool state)
        {
            inCombat = state;
        }
        public float GetMana()
        {
            return _mana.value;
        }

        public float GetMaxMana()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        }

        public float GetRegenRate()
        {
            return GetComponent<BaseStats>().GetStat(Stat.ManaRegRate);
        }
        
        public float GetRegenRateOutOfCombat()
        {
            return GetComponent<BaseStats>().GetStat(Stat.ManaRegRegNiK);
        }
        
        #endregion

        #region Main Methods

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > _mana.value)
            {
                return false;
            }
            _mana.value -= manaToUse;
            return true;
        }
        
        private void RegenerateMana()
        {
            float regenManaPoints = _baseStats.GetStat(Stat.Mana) * (regenerationPercentage * 0.01f);
            _mana.value  = Mathf.Max(_mana.value , regenManaPoints);
        }
        
        public void RestoreMana(float manaToRestore)
        {
            _mana.value  = Mathf.Min(_mana.value + manaToRestore,GetMaxMana());
        }

        #endregion

        #region Saving System Methods

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_mana.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            _mana.value = state.ToObject<float>();
        }

        #endregion
        

    }
}
