using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        private Dictionary<InventoryItem, float> _cooldownTimers = new Dictionary<InventoryItem, float>();
        private Dictionary<InventoryItem, float> _initialCooldownTimes = new Dictionary<InventoryItem, float>();

        #region Basic Unity Methods

        private void Update()
        {
            var keys = new List<InventoryItem>(_cooldownTimers.Keys);
            foreach (var ability in keys)
            {
                _cooldownTimers[ability] -= Time.deltaTime;
                if (_cooldownTimers[ability] <= 0)
                {
                    _cooldownTimers.Remove(ability);
                    _initialCooldownTimes.Remove(ability);
                }
            }
           
        }

        #endregion
        
        #region Main Methods

        public void StartCooldown(InventoryItem ability, float cooldownTime)
        {
            _cooldownTimers[ability] = cooldownTime;
            _initialCooldownTimes[ability] = cooldownTime;
        }

        public float GetCooldownTimeRemaining(InventoryItem ability)
        {
            if (!_cooldownTimers.ContainsKey(ability))
            {
                return 0;
            }
            return _cooldownTimers[ability];
        }

        public float GetCooldownFractionRemaining(InventoryItem ability)
        {
            if (ability == null)
            {
                return 0;
            }
            if (!_cooldownTimers.ContainsKey(ability))
            {
                return 0;
            }

            return _cooldownTimers[ability] / _initialCooldownTimes[ability];
        }
        
        #endregion

       
    }

}