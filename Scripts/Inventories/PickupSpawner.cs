using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{

   public class PickupSpawner : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] InventoryItem item = null;
        [SerializeField] int number = 1;

        #region Basic Unity Methods

        private void Awake()
        {
            // Spawn in Awake so can be destroyed by save system after.
            SpawnPickup();
        }

        #endregion

        #region Main Methods

        private void SpawnPickup()
        {
            var spawnedPickup = item.SpawnPickup(transform.position, number);
            spawnedPickup.transform.SetParent(transform);
        }

        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }

        public bool isCollected() 
        { 
            return GetPickup() == null;
        }
        
        #endregion

        #region Getters/Setters

        public Pickup GetPickup() 
        { 
            return GetComponentInChildren<Pickup>();
        }

        #endregion

        #region SavingSystem Methods
        
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(isCollected());
        }

        public void RestoreFromJToken(JToken state)
        {
            bool shouldBeCollected = state.ToObject<bool>();
            if (shouldBeCollected && !isCollected())
            {
                DestroyPickup();
            }

            if (!shouldBeCollected && isCollected())
            {
                SpawnPickup();
            }
        }
        
        #endregion
        
    }
}