using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New OrientToTargetEffect", menuName = "AbilitySystem/Effects/OrientToTarget", order = 0)]
    public class OrientToTargetEffect : EffectStrategy
    {

        //orients the Player to look at the target
        public override void StartEffect(AbilityData data, Action effectFinished)
        {
            data.GetUser().transform.LookAt(data.GetTargetedPoint());
            effectFinished?.Invoke();
        }

       
    }
}