using System.Collections.Generic;
using RPG.Attributes;
using RPG.Inventories;
using RPG.Stats;
using Unity.Mathematics;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "CombatSystem/Weapons/New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] private float weaponPercentageBonus = 0f;
        [SerializeField] private Weapon equippedWeaponPrefab = null;
        [SerializeField] private AnimatorOverrideController weaponAnimatorOverride = null;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;

        private const string WeaponName = "Weapon";

        #region Main Methods

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;
            if (equippedWeaponPrefab != null)
            {
                var handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedWeaponPrefab, handTransform);
                weapon.gameObject.name = WeaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (weaponAnimatorOverride != null)
            {
                animator.runtimeAnimatorController = weaponAnimatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WeaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(WeaponName);
            }
            if (oldWeapon == null)
            {
                return;
            }
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }
        
        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded)
            {
                handTransform = rightHand;
            }
            else
            {
                handTransform = leftHand;
            }
            return handTransform;
        }

        #endregion

        #region Getters and Setters
        
        public float GetWeaponRange()
        {
            return weaponRange;
        }
        
        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public float GetWeaponPercentageBonus()
        {
            return weaponPercentageBonus;
        }
        
        #endregion

        #region Modifier Methods
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Schaden)
            {
                yield return weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Schaden)
            {
                yield return weaponPercentageBonus;
            }
        }

        public IEnumerable<float> GetBonusModifiers(Stat stat)
        {
            if (GetBonusStat().bonusStat == stat)
            {
                yield return GetBonusStat().currentValue;
            }
        }

        #endregion
        
    }
}
