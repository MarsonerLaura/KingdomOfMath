using System;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far can the pickups be scattered from the dropper.")]
        [SerializeField] private float scatterDistance = 1;
        [SerializeField] private DropLibrary dropLibrary;
        [SerializeField] bool usesCondition = false;
        [SerializeField] Condition completionCondition;
        [SerializeField] private DropLibrary conditionalDropLibrary;

        private const int Attempts = 30;

        private BaseStats _baseStats;
        private GameObject _playerObject;

        #region Basic Unity Methods

        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();
            _playerObject = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            if (usesCondition)
            {
                ChangeDropTableOnCondition();
            }
        }

        #endregion

        #region Main Methods

        private void ChangeDropTableOnCondition()
        {
            if (completionCondition.Check(_playerObject.GetComponents<IPredicateEvaluator>()))
            {
                dropLibrary = conditionalDropLibrary;
            }
        }

        public void RandomDrop()
        {
            if (dropLibrary != null && _baseStats!=null)
            {
                var drops = dropLibrary.GetRandomDrops(_baseStats.GetLevel());
                foreach (var drop in drops)
                {
                    DropItem(drop.item, drop.number);
                }
            }
            else
            {
                Debug.Log("no drop table set or no base stats");
            }
        }

        public void RandomDropFromChest(int level)
        {
            if (dropLibrary != null)
            {
                var drops = dropLibrary.GetRandomDrops(level);
                foreach (var drop in drops)
                {
                    DropItem(drop.item, drop.number);
                }
            }
            else
            {
                Debug.Log("no drop table set ");
            }
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < Attempts; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;
        }
        
        #endregion

        #region Getters/Setters

        public void SetDropTable(DropLibrary newDropLibrary)
        {
            dropLibrary = newDropLibrary;
        }

        public void SetScatterDistance(float value)
        {
            scatterDistance = value;
        }
        
        #endregion
        
    }
}