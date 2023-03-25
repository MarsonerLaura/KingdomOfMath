using System;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New ManaEffect", menuName = "AbilitySystem/Effects/Mana", order = 0)]
    public class ManaEffect : EffectStrategy
    {
        [SerializeField] private float manaChange;
        public override void StartEffect(AbilityData data, Action effectFinished)
        {
            foreach (var target in data.GetTargets())
            {
                Mana mana = target.GetComponent<Mana>();
                if (mana != null)
                {
                    if (manaChange > 0)
                    {
                        mana.RestoreMana(manaChange);
                    }
                    else
                    {
                        //Code for drain Mana if needed
                    }
                }
            }
            effectFinished?.Invoke();
        }
    }

}
