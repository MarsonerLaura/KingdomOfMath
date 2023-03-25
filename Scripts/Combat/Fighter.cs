using System;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using RPG.Movement;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour,IAction//, IJsonSaveable
    {
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeaponConfig = null;
        [SerializeField] private float autoAttackRange = 4f;

        private static readonly int AttackTrigger = Animator.StringToHash("attack");
        private static readonly int StopAttackTrigger = Animator.StringToHash("stopAttack");
        
        private Mover _mover;
        private ActionSheduler _actionSheduler;
        private Animator _animator;
        private BaseStats _baseStats;
        private Health _target;
        private float _timeSinceLastAttack=Mathf.Infinity;
        private WeaponConfig _currentWeaponConfig;
        private LazyValue<Weapon> _currentWeapon;
        private Equipment _equipment;
        
        #region Basic Unity Methods

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _actionSheduler = GetComponent<ActionSheduler>();
            _animator = GetComponent<Animator>();
            _baseStats = GetComponent<BaseStats>();
            _currentWeaponConfig = defaultWeaponConfig;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            _equipment = GetComponent<Equipment>();
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private void Update()
        {
            
            SetCombat();

            _timeSinceLastAttack += Time.deltaTime;
            if (_target == null)
            {
                return;
            }
            if (_target.IsDead())
            {
                _target = FindNewTargetInRange();
                if (_target == null)
                {
                    return;
                }
            }
            if (!IsInRange(_target.transform))
            {
                _mover.MoveTo(_target.transform.position,1f);
            }
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }
        }

        private void OnEnable()
        {
            if (_equipment)
            {
                _equipment.equipmentUpdated += UpdateWeapon;
            }
        }
        
        #endregion

        #region Main Methods
        
        private Health FindNewTargetInRange()
        {
            Health best = null;
            float bestDistance = Mathf.Infinity;
            foreach (var target in FindAllTargetsInRange())
            {
                float targetDistance = Vector3.Distance(transform.position, target.transform.position);
                if (targetDistance < bestDistance)
                {
                    best = target;
                    bestDistance = targetDistance;
                }
            }

            return best;
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up);
            foreach (var hit in raycastHits)
            {
                Health health = hit.transform.GetComponent<Health>();
                if (health == null)
                {
                    continue;
                }
                if (health.IsDead())
                {
                    continue;
                }
                if (health.gameObject.Equals(gameObject))
                {
                    continue;
                }

                yield return health;
            }
        }

        //communicates to health and mana if player is in combat to modify regeneration
        private void SetCombat()
        {
            if (gameObject.tag.Equals("Player"))
            {
                GetComponent<Health>().SetInCombat(_target != null);
                GetComponent<Mana>().SetInCombat(_target != null);
            }
            else if (gameObject.tag.Equals("Enemy"))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<Health>().SetInCombat(_target != null);
                player.GetComponent<Mana>().SetInCombat(_target != null);
            }
        }
        public void Attack(GameObject combatTarget)
        {
            _actionSheduler.StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            _target = null;
            _mover.Cancel();
        }
        
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }

            if (!_mover.CanMoveTo(combatTarget.transform.position) && !IsInRange(combatTarget.transform))
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return combatTarget != null && !targetToTest.IsDead();
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            _currentWeaponConfig = weaponConfig;
            _currentWeapon.value = AttachWeapon(weaponConfig);
        }

        
        private void UpdateWeapon()
        {
            WeaponConfig weapon = _equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeaponConfig);
                return;
            }
            EquipWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            return weaponConfig.Spawn(rightHandTransform,leftHandTransform,_animator);
        }
        
        private void AttackBehaviour()
        {
            transform.LookAt(_target.transform);
            if (timeBetweenAttacks < _timeSinceLastAttack)
            {
                TriggerAttack();
                _timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger(StopAttackTrigger);
            //this will trigger the Hit() Event
            _animator.SetTrigger(AttackTrigger);
        }

        private bool IsInRange(Transform targetTransform)
        {
            return Vector3.Distance(targetTransform.position, transform.position) < _currentWeaponConfig.GetWeaponRange();
        }
        
        private void StopAttack()
        {
            _animator.ResetTrigger(AttackTrigger);
            _animator.SetTrigger(StopAttackTrigger);
        }
        
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }

        public Transform GetHandTransform(bool isRightHand)
        {
            if (isRightHand)
            {
                return rightHandTransform;
            }
            else
            {
                return leftHandTransform;
            }
        }
        
        #endregion

        #region Getters/Setters

        public Health GetTarget()
        {
            return _target;
        }

        #endregion
        
        #region Animation Event Methods
        
        void Hit()
        {
            if (_target == null)
            {
                return;
            }
            float damage = _baseStats.GetStat(Stat.Schaden);

            BaseStats targetBaseStats = _target.GetComponent<BaseStats>();
            if (targetBaseStats != null)
            {
                float defense = targetBaseStats.GetStat(Stat.Abwehr);
                damage /= 1 + defense / damage;
            }


            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }
            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(rightHandTransform,leftHandTransform,_target, gameObject, damage);
            }
            else
            {
                _target.TakeDamage(gameObject, damage);
            }
        }

        void Shoot()
        {
            Hit();
        }
        
        #endregion
        
        #region SavingSystem Methods

       /* public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentWeaponConfig.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            string weaponName = state.ToObject<string>();
            WeaponConfig weaponConfig = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weaponConfig);
        }*/

        #endregion
        
    }
}