using System;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using RPG.Inventories;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable, IItemStore
    {
        [SerializeField] private float experiencePoints = 0;
        
        public event Action onExperienceGained;
        
        
        #region Main Methods

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained?.Invoke();
        }

        public int AddItems(InventoryItem item, int number)
        {
            if (item is ExperienceItem award)
            {
                GainExperience(award.GetExperienceToAward() * number); //Edited
                return number;
            }
            return 0;
        }

        #endregion

        #region SavingSystem Methods

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }
        
        public void RestoreFromJToken(JToken state)
        {
            experiencePoints = state.ToObject<float>();
        }

        #endregion

        #region Getters/Setters

        public float GetExperiencePoints()
        {
            return experiencePoints;
        }

        #endregion
       
    }
}
