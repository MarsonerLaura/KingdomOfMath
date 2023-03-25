using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using RPG.Stats;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "AbilitySystem/Ability", order = 0)]
    public class Ability : ActionItem, IModifierProvider
    {
        [SerializeField] private TargetingStrategy targetingStrategy;
        [SerializeField] private FilteringStrategy[] filteringStrategies;
        [SerializeField] private EffectStrategy [] effectStrategies;
        [SerializeField] private float cooldownTime;
        [SerializeField] private float manaCost;

        public override bool Use(GameObject user)
        {
            Mana mana = user.GetComponent<Mana>();
            if (mana.GetMana() < manaCost)
            {
                return false;
            }

            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if (cooldownStore.GetCooldownTimeRemaining(this) > 0)
            {
                return false;
            }

            AbilityData data = new AbilityData(user);

            ActionSheduler actionSheduler = user.GetComponent<ActionSheduler>();
            actionSheduler.StartAction(data);
            
            targetingStrategy.StartTargeting(data, ()=>
            {
                TargetsAquired(data);
            });
            return true;
        }

        private void TargetsAquired(AbilityData data)
        {
            if (data.IsCancelled())
            {
                return;
            }
            Mana mana = data.GetUser().GetComponent<Mana>();
            if (!mana.UseMana(manaCost))
            {
                return;
            }
            
            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, cooldownTime);
            
            foreach (var filteringStrategy in filteringStrategies)
            {
                data.SetTargets(filteringStrategy.Filter(data.GetTargets()));
            }

            foreach (var effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
                
            }
        }

        private void EffectFinished()
        {
            
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            yield break;
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            yield break;
        }

        
        public IEnumerable<float> GetBonusModifiers(Stat stat)
        {
            if (GetBonusStat().bonusStat == stat)
            {
                yield return GetBonusStat().currentValue;
            }
        }
        
    }
}

