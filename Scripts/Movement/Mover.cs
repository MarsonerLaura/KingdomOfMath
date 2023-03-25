using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float maxNavPathLength = 40f;
        
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        
        private NavMeshAgent _agent;
        private Animator _animator;
        private ActionSheduler _actionScheduler;
        private Health _health;

        #region Base Methods

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionSheduler>();
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            if (_health != null)
           {
               _agent.enabled = !_health.IsDead();
           }
           else
           {
               _agent.enabled = true;
           }
           UpdateAnimator();
        }

        #endregion

        #region Main Methods
        
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _agent.destination = destination;
            _agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            _agent.isStopped = false;
        }
        
        public void StartMoveAction(Vector3 destination,float speedFraction)
        {
            _actionScheduler.StartAction(this);
            MoveTo(destination,speedFraction);
        }

        public void Cancel()
        {
            _agent.isStopped = true;
        }
        
        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath)
            {
                return false;
            }
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            if (GetPathLength(path) > maxNavPathLength)
            {
                return false;
            }
            return true;
        }
        
        
        private void UpdateAnimator()
        {
            if (_animator == null || _animator.runtimeAnimatorController==null)
            {
                return;
            }
            Vector3 velocity = _agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            _animator.SetFloat(ForwardSpeed, speed);
        }

        //calculates the length of the input navmesh path
        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2)
            {
                return total;
            }

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }
        
        #endregion

        #region SavingSystem Methods

            public JToken CaptureAsJToken()
            {
                return transform.position.ToToken();
            }

            public void RestoreFromJToken(JToken state)
            {
                _agent.enabled = false;
                transform.position = state.ToVector3();
                _agent.enabled = true;
                _actionScheduler.CancelCurrentAction();
            }

        #endregion

    }
}
