using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine.SceneManagement;

namespace RPG.Inventories
{
    public class ItemDropper : MonoBehaviour, IJsonSaveable
    {
        private List<Pickup> droppedItems = new List<Pickup>();
        
        class otherSceneDropRecord
        {
            public string id;
            public int number;
            public Vector3 location;
            public int scene;
        }

        private List<otherSceneDropRecord> otherSceneDrops = new List<otherSceneDropRecord>();

        #region Main Methods
        
        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, GetDropLocation(), number);
        }
        
        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation(), 1);
        }
        
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }
        
        public void SpawnPickup(InventoryItem item, Vector3 spawnLocation, int number)
        {
            var pickup = item.SpawnPickup(spawnLocation, number);
            droppedItems.Add(pickup);
        }
        
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }
        private void ClearExistingDrops()
        {
            foreach (var oldDrop in droppedItems)
            {
                if (oldDrop != null) Destroy(oldDrop.gameObject);
            }

            otherSceneDrops.Clear();
        }

        List<otherSceneDropRecord> MergeDroppedItemsWithOtherSceneDrops()
        {
            List<otherSceneDropRecord> result = new List<otherSceneDropRecord>();
            result.AddRange(otherSceneDrops);
            foreach (var item in droppedItems)
            {
                otherSceneDropRecord drop = new otherSceneDropRecord();
                drop.id = item.GetItem().GetItemID();
                drop.number = item.GetNumber();
                drop.location = item.transform.position;
                drop.scene = SceneManager.GetActiveScene().buildIndex;
                result.Add(drop);
            }
            return result;
        }
        
        #endregion

        #region SavingSystem Methods

        public JToken CaptureAsJToken()
        {
            RemoveDestroyedDrops();
            var drops = MergeDroppedItemsWithOtherSceneDrops();
            JArray state = new JArray();
            IList<JToken> stateList = state;
            foreach (var drop in drops)
            {
                JObject dropState = new JObject();
                IDictionary<string, JToken> dropStateDict = dropState;
                dropStateDict["id"] = JToken.FromObject(drop.id);
                dropStateDict["number"] = drop.number;
                dropStateDict["location"] = drop.location.ToToken();
                dropStateDict["scene"] = drop.scene;
                stateList.Add(dropState);
            }

            return state;
        }
        
        public void RestoreFromJToken(JToken state)
        {
            if (state is JArray stateArray)
            {
                int currentScene = SceneManager.GetActiveScene().buildIndex;
                IList<JToken> stateList = stateArray;
                ClearExistingDrops();
                foreach (var entry in stateList)
                {
                    if (entry is JObject dropState)
                    {
                        IDictionary<string, JToken> dropStateDict = dropState;
                        int scene = dropStateDict["scene"].ToObject<int>();
                        InventoryItem item = InventoryItem.GetFromID(dropStateDict["id"].ToObject<string>());
                        int number = dropStateDict["number"].ToObject<int>();
                        Vector3 location = dropStateDict["location"].ToVector3();
                        if (scene == currentScene)
                        {
                            SpawnPickup(item, location, number);
                        }
                        else
                        {
                            var otherDrop = new otherSceneDropRecord();
                            otherDrop.id = item.GetItemID();
                            otherDrop.number = number;
                            otherDrop.location = location;
                            otherDrop.scene = scene;
                            otherSceneDrops.Add(otherDrop);
                        }
                    }
                }
            }
        }

        #endregion
        
    }

}