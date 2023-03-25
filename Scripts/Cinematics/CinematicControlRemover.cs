using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private GameObject _player = null;
        private PlayableDirector _playableDirector = null;
        private PlayerInput _playerInput;
        
        #region Basic Unity Methods

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _playableDirector = GetComponent<PlayableDirector>();
        }
        
        private void OnEnable()
        {
            _playableDirector.played += DisableControl;
            _playableDirector.stopped += EnableControl;
        }

        private void OnDisable()
        {
            _playableDirector.played -= DisableControl;
            _playableDirector.stopped -= EnableControl;
        }
        
        #endregion
        
        #region Main Methods

        private void DisableControl(PlayableDirector playableDirector)
        {
            _playerInput = InputManager.Instance.GetPlayerInput();
            _playerInput.Disable();
            _player.GetComponent<ActionSheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = false;
        }

        private void EnableControl(PlayableDirector playableDirector)
        {
            _playerInput = InputManager.Instance.GetPlayerInput();
            _playerInput.Enable();
            _player.GetComponent<PlayerController>().enabled = true;
        }

        #endregion
        
    }
}
