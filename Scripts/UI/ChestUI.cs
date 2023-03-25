using RPG.Chests;
using RPG.Control;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace RPG.UI
{
    public class ChestUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private TMP_InputField answerInputField;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Button quitButton;

        private ChestOpener _playerChestOpener;
        private PlayerInput _playerInput;

        #region Basic Unity Methods

        private void Awake()
        {
            _playerChestOpener = GameObject.FindGameObjectWithTag("Player").GetComponent<ChestOpener>();
            
        }

        private void OnEnable()
        {
            _playerInput = InputManager.Instance.GetPlayerInput();
            _playerInput.Disable();
            _playerChestOpener.OnChestUpdated += UpdateUI;
            _playerChestOpener.OnUnlockFailed += UnlockFailed;
        }

        private void OnDisable()
        {
            _playerInput.Enable();
        }

        private void Start()
        {
            unlockButton.onClick.AddListener(() => _playerChestOpener.TryToUnlock(answerInputField.text));
            quitButton.onClick.AddListener(() => _playerChestOpener.CloseChest());
            UpdateUI();
        }
        
        #endregion

        #region Main Methods

        private void UnlockFailed()
        {
            answerInputField.text = "";
        }

        private void UpdateUI()
        {
            gameObject.SetActive(_playerChestOpener.IsOpen());
            if (!_playerChestOpener.IsOpen())
            {
                return;
            }

            questionText.text = _playerChestOpener.GetQuestion();
            answerInputField.text = "";

        }
        
        #endregion
        
    }
}
