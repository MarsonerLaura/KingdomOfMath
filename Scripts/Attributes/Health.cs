using System;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
       
        [SerializeField] private float regenerationPercentage = 70f;
        [SerializeField] private TakeDamageEvent takeDamage;
        [SerializeField] public UnityEvent onDie;
        
        private static readonly int DieTrigger = Animator.StringToHash("die");
        
        private LazyValue<float> _healthPoints; //lazyvalue ensures that variable is initialized before use
        private bool _wasDeadLastFrame = false;
        private BaseStats _baseStats = null;
        private ActionSheduler _actionScheduler = null;
        private Animator _animator = null;
        bool _inCombat;

        #region Basic Unity Methods

        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();
            _actionScheduler = GetComponent<ActionSheduler>();
            _animator = GetComponent<Animator>();
            _healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void Update()
        {
            if (gameObject.CompareTag("Player"))
            {
                if (_inCombat == false && IsDead() == false)
                {
                    _healthPoints.value += GetRegenRateOutOfCombat() * Time.deltaTime;
                    if (_healthPoints.value > GetMaxHealthPoints())
                    {
                        _healthPoints.value = GetMaxHealthPoints();
                    }
                }
            }
        }

        private float GetInitialHealth()
        {
            return _baseStats.GetStat(Stat.Leben);
        }
        private void Start()
        {
            _healthPoints.ForceInit();
        }

        private void OnEnable()
        {
            _baseStats.onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            _baseStats.onLevelUp -= RegenerateHealth;
        }

        #endregion

        #region Main Methods
        
        public void TakeDamage(GameObject instigator, float damage)
        {
            _healthPoints.value  = Mathf.Max(_healthPoints.value - damage, 0);
            if (IsDead())
            {
                onDie.Invoke();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
            UpdateState();
        }
        
        public float GetMaxHealthPoints()
        {
            return _baseStats.GetStat(Stat.Leben);
        }
        
        
        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return _healthPoints.value / _baseStats.GetStat(Stat.Leben);
        }
        
        public void Heal(float healthToRestore)
        {
            _healthPoints.value  = Mathf.Min(_healthPoints.value + healthToRestore,GetMaxHealthPoints());
            UpdateState();
        }
        
        
        private void RegenerateHealth()
        {
            float regenHealthPoints = _baseStats.GetStat(Stat.Leben) * (regenerationPercentage * 0.01f);
            _healthPoints.value  = Mathf.Max(_healthPoints.value , regenHealthPoints);
        }
        
        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null)
            {
                return;
            }
            experience.GainExperience(_baseStats.GetStat(Stat.XPBelohnung));
        }

        private void UpdateState()
        {
            if (!_wasDeadLastFrame && IsDead())
            {
                _animator.SetTrigger(DieTrigger);
                _actionScheduler.CancelCurrentAction();
            }

            if (_wasDeadLastFrame && !IsDead())
            {
                _animator.Rebind();
            }

            _wasDeadLastFrame = IsDead();
        }
        
        public float GetRegenRateOutOfCombat()
        {
            return GetComponent<BaseStats>().GetStat(Stat.LebensRegNiK);
        }
        
        public bool IsDead()
        {
            return _healthPoints.value <= 0;
        }
        
        #endregion

        #region Getters/Setters
        public void SetInCombat(bool state)
        {
            _inCombat=state;
        }
        public float GetHealthPoints()
        {
            return _healthPoints.value ;
        }

        #endregion

        #region SavingSystem Methods

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_healthPoints.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            float val = state.ToObject<float>();
            _healthPoints.value = val;
            UpdateState();
        }

        #endregion

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
            
        }

        
    }
}
