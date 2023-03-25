using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Chests
{
    public class ChestSpawner : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] ChestItem chestItem = null;

        #region Basic Unity Methods

        private void Awake()
        {
            SpawnChest();
        }

        #endregion

        #region Main Methods

        public bool WasAlreadyOpened() 
        { 
            return GetChest() == null;
        }
        
        private void SpawnChest()
        {
            var spawnedPickup = chestItem.SpawnChest(transform.position);
            spawnedPickup.transform.SetParent(transform);
        }

        private void DestroyPickup()
        {
            if (GetChest())
            {
                Destroy(GetChest().gameObject);
            }
        }

        #endregion

        #region Getters/Setters

        public Chest GetChest() 
        { 
            return GetComponentInChildren<Chest>();
        }

        #endregion

        #region SavingSystem Methods

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(WasAlreadyOpened());
        }

        public void RestoreFromJToken(JToken state)
        {
            bool shouldBeOpened = state.ToObject<bool>();
            if (shouldBeOpened && !WasAlreadyOpened())
            {
                DestroyPickup();
            }

            if (!shouldBeOpened && WasAlreadyOpened())
            {
                SpawnChest();
            }

        }

        #endregion
        
    }
}