using System;
using RPG.Control;
using RPG.SceneManagement;
using UnityEngine;

namespace RPG.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        private PlayerController _playerController;
        private PlayerInput _playerInput;

        #region Basic Unity Methods

        private void Awake()
        {
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            if (_playerController == null)
            {
                return;
            }
            Time.timeScale = 0;
            _playerInput = InputManager.Instance.GetPlayerInput();
            _playerInput.Disable();
            _playerController.enabled = false;
        }

        private void OnDisable()
        {
            if (_playerController == null)
            {
                return;
            }
            _playerInput.Enable();
            Time.timeScale = 1;
            _playerController.enabled = true;
        }

        #endregion

        #region Main Methods

        public void Save()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
        }

        public void SaveAndQuit()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            savingWrapper.LoadMenu();
        }

        #endregion

    }
}