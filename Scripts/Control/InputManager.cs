using UnityEngine;

namespace RPG.Control
{

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        
        private PlayerInput _playerInput;

        #region Basic Unity Methods

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
            Instance = this;
        }

        #endregion

        #region Getters/Setters

        public PlayerInput GetPlayerInput()
        {
            return _playerInput;
        }

        #endregion
        
    }
}
