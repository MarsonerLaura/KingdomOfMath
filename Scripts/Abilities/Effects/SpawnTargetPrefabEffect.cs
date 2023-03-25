using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "New SpawnTargetPrefabEffect", menuName = "AbilitySystem/Effects/SpawnTargetPrefab", order = 0)]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] private Transform prefabToSpawn;
        [SerializeField] private float destroyDelay = -1;
        
        public override void StartEffect(AbilityData data, Action effectFinished)
        {
            data.StartCoroutine(Effect(data, effectFinished));
        }
        
        private IEnumerator Effect(AbilityData data, Action effectFinished)
        {
            Transform instance = Instantiate(prefabToSpawn);
            instance.position = data.GetTargetedPoint();
            if (destroyDelay > 0)
            {
                yield return new WaitForSeconds(destroyDelay);
                Destroy(instance.gameObject);
            }
            effectFinished?.Invoke();
        }
    }
}