using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "New ExperienceRewardItem", menuName = "InventorySystem/ExperienceReward Item", order = 0)]

    public class ExperienceItem : InventoryItem
    {
        [SerializeField] int experienceToAward;

        public int GetExperienceToAward() => experienceToAward;
    }
}
