using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New TriggerAnimationEffect", menuName = "AbilitySystem/Effects/TriggerAnimation", order = 0)]
    public class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] private string animationTrigger;
        
        public override void StartEffect(AbilityData data, Action effectFinished)
        {
            Animator animator = data.GetUser().GetComponent<Animator>();
            animator.SetTrigger(animationTrigger);
            effectFinished?.Invoke();
        }

       
    }
}