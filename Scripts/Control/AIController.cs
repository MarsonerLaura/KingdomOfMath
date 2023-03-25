using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 5f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float waypointDwellTime = 2f;
        [SerializeField][Range(0f,1f)] private float patrolSpeedFraction = 0.2f;
        [SerializeField] private float aggroCooldownTime = 5f;
        [SerializeField] private float shoutDistance = 5f; //distance the enemy aggravates other enemies in range when attacked

        private GameObject _player;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;
        private ActionSheduler _actionScheduler;
        private LazyValue<Vector3> _guardPosition;
        private int _currentWaypointIndex = 0;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float timeSinceAggravated = Mathf.Infinity;

        #region Basic Unity Methods

        private void Awake()
        {
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionSheduler>();
            _player = GameObject.FindWithTag("Player");
            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
            _guardPosition.ForceInit();
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDead())
            {
                return;
            }
            if (IsAggravated() && _fighter.CanAttack(_player))
            {
                AttackBehaviour();
            }
            else if(_timeSinceLastSawPlayer<suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();
        }
        

        #endregion
        
        #region Unity Event Methods

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position,chaseDistance);
        }
        
        #endregion
        
        #region Main Methods

        //if enemy gets hit gets aggravated
        public void Aggravate()
        {
            timeSinceAggravated = 0;
        }


        //true if enemy is in range of player or got aggravated because itself or another enemy got hit
        private bool IsAggravated()
        {
            float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance || timeSinceAggravated < aggroCooldownTime;
        }
        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition.value;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    CycleWaypoint();
                    _timeSinceArrivedAtWaypoint = 0;
                }
                
                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                _mover.StartMoveAction(nextPosition,patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint <= waypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);
            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController aiController = hit.transform.GetComponent<AIController>();
                if (aiController == null)
                {
                    continue;
                }
                aiController.Aggravate();
            }
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }
        
        public void Reset()
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.Warp(_guardPosition.value); 
            _currentWaypointIndex = 0; 
            _timeSinceLastSawPlayer = Mathf.Infinity;
            _timeSinceArrivedAtWaypoint = Mathf.Infinity;
            timeSinceAggravated = Mathf.Infinity;
        }
        
        #endregion
        
    }
}
