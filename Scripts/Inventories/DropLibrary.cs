using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("InventorySystem/Drop Library"))]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField] private DropConfig[] potentialDrops;
        [SerializeField] private float [] dropChancePercentage;
        [SerializeField] private int[] minDrops;
        [SerializeField] private int[] maxDrops;
        
        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;

            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable())
                {
                    return 1;
                }
                return Random.Range(GetByLevel(minNumber, level), GetByLevel(maxNumber, level) + 1);
            }
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }
        

        #region Main Methods

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }

            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        private Dropped GetRandomDrop(int level)
        {
            DropConfig drop = SelectRandomItem(level);
            Dropped result = new Dropped();
            result.item = drop.item;
            result.number = drop.GetRandomNumber(level);
            return result;
        }

        private int GetRandomNumberOfDrops(int level)
        {
            int min = GetByLevel(minDrops, level);
            int max = GetByLevel(maxDrops, level);
            return Random.Range(min, max);
        }

        private bool ShouldRandomDrop(int level)
        {
            return Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
        }

        DropConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance);
            float addedChances = 0;
            foreach (DropConfig drop in potentialDrops)
            {
                addedChances += GetByLevel(drop.relativeChance,level);
                if (addedChances > randomRoll)
                {
                    return drop;
                }
            }
            return null;
        }

        private float GetTotalChance(int level)
        {
            float total = 0;
            foreach (DropConfig drop in potentialDrops)
            {
                total += GetByLevel(drop.relativeChance,level);
            }

            return total;
        }
        
        static T GetByLevel<T>(T[]values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }

            if (level > values.Length)
            {
                return values[values.Length - 1];
            }

            if (level <= 0)
            {
                return default;
            }

            return values[level - 1];
        }

        #endregion
        
    }
}