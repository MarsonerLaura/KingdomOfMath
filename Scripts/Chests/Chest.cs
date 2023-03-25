using System;
using RPG.Control;
using RPG.Inventories;
using UnityEngine;
using UnityEngine.InputSystem;
using RPG.Movement;

namespace RPG.Chests
{

    public class Chest : MonoBehaviour, IRaycastable
    {
        ChestItem chestItem;
        private PlayerController _targetController;

        #region Basic Unity Methods

        private void Update()
        {
            if (_targetController && Vector3.Distance(_targetController.transform.position, this.transform.position) < 5)
            {
                _targetController.GetComponent<Mover>().Cancel();
                _targetController.GetComponent<ChestOpener>().InteractWithChest(this, chestItem);
                _targetController = null;

            }
        }

        #endregion

        #region Main Methods

        public void Setup(ChestItem item)
        {
            chestItem = item;
        }

        public void InteractWithChest()
        {
            //TODO spawn talking fairy
            PlayVoiceLine();
        }

        private void PlayVoiceLine()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = chestItem.GetVoice();
            audioSource.Play();
        }

        //called after chest was unlocked to destroy chest object
        public void LootChest()
        {
            //play animation,....
            DropItems();

            Destroy(gameObject);
        }

        private void DropItems()
        {
            RandomDropper randomDropper = GetComponent<RandomDropper>();
            randomDropper.SetScatterDistance(0);
            randomDropper.SetDropTable(chestItem.GetDropTable());
            randomDropper.RandomDropFromChest(chestItem.GetLevel());
        }
        
        //Can be used in the futor for maybe a level restriction
        public bool CanBeOpened()
        {
            return true;
        }
        
        public CursorType GetCursorType()
        {
            return CursorType.Chest;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (chestItem == null)
            {
                Debug.Log("ChestItem not set.");
                return false;
            }
            if (Mouse.current.leftButton.isPressed)
            {
                _targetController = callingController;
                if (Vector3.Distance(_targetController.transform.position, this.transform.position) > 5)
                {
                    _targetController.GetComponent<Mover>().StartMoveAction(this.transform.position, 1);
                }
                else
                {
                    _targetController.GetComponent<Mover>().Cancel();
                    _targetController.GetComponent<ChestOpener>().InteractWithChest(this, chestItem);
                    _targetController = null;
                }
                
            }
            return true;
        }
        #endregion
        
    }
}