using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New DelayCompositeEffect", menuName = "AbilitySystem/Effects/DelayComposite", order = 0)]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] private EffectStrategy[] effectsToDelay;
        [SerializeField] private float delay;
        [SerializeField] private bool abortIfCancelled = false;

        public override void StartEffect(AbilityData data, Action effectFinished)
        {
            data.StartCoroutine(DelayEffect(data, effectFinished));
        }

        private IEnumerator DelayEffect(AbilityData data, Action effectFinished)
        {
            if (data.IsCancelled() && abortIfCancelled)
            {
                yield break;
            }

            yield return new WaitForSeconds(delay);
            foreach (var effect in effectsToDelay)
            {
                effect.StartEffect(data, effectFinished);
            }
        }
    }
}