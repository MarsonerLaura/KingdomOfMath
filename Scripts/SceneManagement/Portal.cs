using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        private enum DestinationIdentifier
        {
            A,
            B,
            C,
            D,
            E
        }
        
        [SerializeField] private int sceneToLoadIndex=-1;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private DestinationIdentifier destination;
        [SerializeField] private float fadeOutTime = 0.5f;
        [SerializeField] private float fadeInTime = 0.5f;
        [SerializeField] private float fadeWaitTime = 0.25f;

        #region Unity Event Methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(TransitionToScene());
            }
        }

        #endregion

        #region Main Methods

        private IEnumerator TransitionToScene()
        {
            if (sceneToLoadIndex < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break; 
            }
            
            DontDestroyOnLoad(gameObject);
            
            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();

            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            
            yield return fader.FadeOut(fadeOutTime);
            
            savingWrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoadIndex);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            
            savingWrapper.Load();
 
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            savingWrapper.Save();
            
            yield return new WaitForSeconds(fadeWaitTime);
            
            fader.FadeIn(fadeInTime);
            
            newPlayerController.enabled = true;
            
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            print("portal: "+otherPortal);
            GameObject player = GameObject.FindWithTag("Player");
            print("player: "+player);
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in Resources.FindObjectsOfTypeAll<Portal>())
            {
                if (portal == this)
                {
                    continue;
                }

                if (portal.destination != this.destination)
                {
                    continue;
                }
                return portal;
            }

            return null;
        }

        #endregion
        
    }
}