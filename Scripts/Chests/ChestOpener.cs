using System;
using RPG.CameraEffects;
using UnityEngine;

namespace RPG.Chests
{

    public class ChestOpener : MonoBehaviour
    {
        //[SerializeField] private ScreenShakeEffect screenShake;
        
        private Chest _currentChest;
        private ChestItem _currentChestItem;
        public event Action OnChestUpdated;
        public event Action OnUnlockFailed;
        private float shakeIntensity=1f;
        private float shakeTime=0.3f;

        #region Main Methods

        public void InteractWithChest(Chest chest, ChestItem chestItem)
        {
            if (_currentChestItem != chestItem)
            {
                _currentChest = chest;
                _currentChestItem = chestItem;
                _currentChest.InteractWithChest();
                OnChestUpdated?.Invoke();  
            }
        }

        public void CloseChest()
        {
            _currentChest = null;
            _currentChestItem = null;
            OnChestUpdated?.Invoke();
        }
        
        public bool IsOpen()
        {
            return _currentChestItem != null;
        }

        public void TryToUnlock(string answer)
        {
            if (_currentChestItem.CheckAnswer(answer))
            {
                _currentChest.LootChest();
                CloseChest();
            }
            else
            {
                //TODO add Screenshake Effect
                OnUnlockFailed?.Invoke();
            }
        }
        
        #endregion

        #region Getters/Setters

        public string GetQuestion()
        {
            return _currentChestItem.GetQuestion();
        }

        #endregion

    }
}
