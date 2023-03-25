using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] private float speed = 1f;
        [SerializeField] private bool isHoming = true;
        [SerializeField] private float maxLifeTime = 10f;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private float lifeAfterImpact = 0.5f;
        [SerializeField] private UnityEvent onHit;
       
        private Health _target = null;
        private float _weaponDamage = 0f;
        private GameObject _instigator = null;
        private Vector3 _targetPoint;

        #region Basic Unity Methods

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (_target != null && isHoming && !_target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        #endregion

        #region Unity Event Methods

        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();
            if (_target != null && health != _target)
            {
                return;
            }
            if (health == null || health.IsDead())
            {
                return;
            }
            if (other.gameObject == _instigator)
            {
                return;
            }
            health.TakeDamage(_instigator, _weaponDamage);
            speed = 0;
            onHit.Invoke();
            if (hitEffect != null)
            {
                //Instantiate(hitEffect, GetAimLocation(),transform.rotation);
                Instantiate(hitEffect, transform.position, transform.rotation);
            }
            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            Destroy(gameObject,lifeAfterImpact);
        }

        #endregion
        
        #region Main Methods

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, target);
        }

        public void SetTarget(Vector3 targetPoint, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, null, targetPoint);
        }

        public void SetTarget(GameObject instigator, float damage, Health target=null, Vector3 targetPoint=default)
        {
            _target = target;
            _targetPoint = targetPoint;
            _weaponDamage = damage;
            _instigator = instigator;

            Destroy(gameObject, maxLifeTime);
        }
        
        private Vector3 GetAimLocation()
        {
            if (_target == null)
            {
                return _targetPoint;
            }
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return _target.transform.position;
            }
            return _target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        #endregion
        
    }
}