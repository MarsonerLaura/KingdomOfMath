using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool _wasTriggered = false;
        private PlayableDirector _playableDirector = null;
        
        #region Basic Unity Methods

        private void Awake()
        {
            _playableDirector = GetComponent<PlayableDirector>();
        }
        
        #endregion

        #region Unity Event Methods

        private void OnTriggerEnter(Collider other)
        {
            if (!_wasTriggered && other.CompareTag("Player"))
            {
                _wasTriggered = true;
                _playableDirector.Play();
                //TODO maybe destroy gameobject
            }
        }

        #endregion
        
    }
}
