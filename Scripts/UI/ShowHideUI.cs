using RPG.Control;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] GameObject uiInventoryContainer = null;
        [SerializeField] GameObject uiEquipmentContainer = null;
        [SerializeField] GameObject uiQuestsContainer = null;
        [SerializeField] GameObject uiTraitsContainer = null;
        [SerializeField] GameObject uiPauseContainer = null;
        [SerializeField] GameObject uiStatsContainer = null;
        [SerializeField] private GameObject uiHUD;
        [SerializeField] private GameObject uiPurse;
        private PlayerInput _playerInput;

        #region Basic Unity Methods

        public void OnEnable()
        {
            _playerInput = InputManager.Instance.GetPlayerInput();
            _playerInput.Enable();
            _playerInput.Player.Inventory.performed += ShowInventoryUI;
            _playerInput.Player.Quests.performed += ShowQuestUI;
            _playerInput.Player.Traits.performed += ShowTraitsUI;
            _playerInput.Player.Pause.performed += ShowPauseUI;
            _playerInput.Player.Stats.performed += ShowStatUI;
        }
        
        public void OnDisable()
        {
            _playerInput.Player.Inventory.performed -= ShowInventoryUI;
            _playerInput.Player.Quests.performed -= ShowQuestUI;
            _playerInput.Player.Traits.performed -= ShowTraitsUI;
            _playerInput.Player.Pause.performed -= ShowPauseUI;
            _playerInput.Player.Stats.performed -= ShowStatUI;
        }

        private void OnDestroy()
        {
            _playerInput.Disable();
        }
        
        void Start()
        {
            uiInventoryContainer.SetActive(false);
            uiQuestsContainer.SetActive(false);
            uiTraitsContainer.SetActive(false);
            uiPauseContainer.SetActive(false);
            uiStatsContainer.SetActive(false);
            uiEquipmentContainer.SetActive(false);
        }

        #endregion

        #region Main Methods

        private void ShowTraitsUI(InputAction.CallbackContext obj)
        {
            uiTraitsContainer.SetActive(!uiTraitsContainer.activeSelf);
        }
        
        private void ShowPauseUI(InputAction.CallbackContext obj)
        {
            uiPauseContainer.SetActive(!uiPauseContainer.activeSelf);
        }

        public void ShowQuestUI()
        {
            uiQuestsContainer.SetActive(!uiQuestsContainer.activeSelf);
        }

        private void ShowQuestUI(InputAction.CallbackContext obj)
        {
            ShowQuestUI();
        }
        
        private void ShowInventoryUI(InputAction.CallbackContext obj)
        {
            bool inv = uiInventoryContainer.activeSelf;
            bool equ = uiEquipmentContainer.activeSelf;
            if ((inv && equ) || (!inv && !equ))
            {
                uiInventoryContainer.SetActive(!inv);
                uiEquipmentContainer.SetActive(!equ);
            }
            else if (inv)
            {
                uiEquipmentContainer.SetActive(true);
            }
            else
            {
                uiInventoryContainer.SetActive(true);
            }
        }

        private void ShowStatUI(InputAction.CallbackContext obj)
        {
            uiStatsContainer.SetActive(!uiStatsContainer.activeSelf);
        }
        
        public void ShowHUD()
        {
            uiHUD.SetActive(!uiHUD.activeSelf);
        }
        
        public void ShowPurse()
        {
            uiPurse.SetActive(!uiPurse.activeSelf);
        }

        #endregion
        
    }
}