using System.Collections;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig weaponConfig;
        [SerializeField] private float healthToRestore = 0;
        [SerializeField] private float respawnTime = 5f;

        private Collider _collider;
        
        #region Basic Unity Methods

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        #endregion
        
        #region Unity Event Methods

        private void OnTriggerEnter(Collider other)
        {
            if (!other.tag.Equals("Player"))
            {
                return;
            }
            Pickup(other.gameObject);
            //Destroy(gameObject);
        }
        
        #endregion

        #region Main Methods

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Pickup(callingController.gameObject);
            }
            return true;
        }
        
        
        private void Pickup(GameObject subject)
        {
            if (weaponConfig != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weaponConfig);
            }

            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            _collider.enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        #endregion
        
    }
}
