using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("InventorySystem/Stats Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] private Modifier[] additiveModifiers;
        [SerializeField] private Modifier[] percentageModifiers;

        [System.Serializable]
        public struct Modifier
        {
            public Stat stat;
            public float value;
        }
        
        #region Modifier Methods
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (Modifier modifier in additiveModifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return modifier.value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (Modifier modifier in percentageModifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return modifier.value;
                }
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
