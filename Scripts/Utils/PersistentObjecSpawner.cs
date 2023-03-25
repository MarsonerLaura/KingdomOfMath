using UnityEngine;

namespace RPG.Utils
{
    /*
     * An alternative to using the singleton pattern.
     * Will handle spawning a prefab only once across multiple scenes.
     * Place this in a prefab that exists in every scene.
     * Point to another prefab that contains all GameObjects that should be singletons.
     * The class will spawn the prefab only once and set it to persist between scenes.
     */
    public class PersistentObjecSpawner : MonoBehaviour
    {
        [Tooltip("This prefab will only be spawned once and persisted between " +
                 "scenes.")]
        [SerializeField] private GameObject persistentObjectPrefab;
        private static bool _hasSpawned = false;

        #region Basic Unity Methods

        private void Awake()
        {
            if (_hasSpawned)
            {
                return;
            }
            SpawnPersistentObjects();
            _hasSpawned = true;
        }

        #endregion

        #region Main Methods

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }

        #endregion
        
    }
}
