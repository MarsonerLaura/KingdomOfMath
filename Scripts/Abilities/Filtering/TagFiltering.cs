using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filtering
{
    [CreateAssetMenu(fileName = "New TagFiltering", menuName = "AbilitySystem/Filtering/Tag", order = 0)]
    public class TagFiltering : FilteringStrategy
    {
        [SerializeField] private string tagToFilter;
        
        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
        {
            foreach (var obj in objectsToFilter)
            {
                if (obj.CompareTag(tagToFilter))
                {
                    yield return obj;
                }
            }
        }
    }
}

