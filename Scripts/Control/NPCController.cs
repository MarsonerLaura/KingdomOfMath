
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using RPG.Dialogue;
using RPG.Saving;
using Newtonsoft.Json.Linq;

namespace RPG.Control
{

    public class NPCController : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private NPCAction[] npcActions;

        [System.Serializable]
        public class NPCAction
        {
            public string description;
            public Transform destination;
            public float timeInAction = Mathf.Infinity;
            public float speedToThisAction = 2;
            public Dialogue.Dialogue dialogue;
            public string[] animationStateNames;
            public float[] timeBetweenAnimations;

        }
        
        private NavMeshAgent _agent;
        private Mover _mover;
        private int _currentActionIndex;
        private NPCAction _currentAction;
        private float _timePassedInAction;
        private Animator _animator;
        private bool _inExecution = false;
        private AIConversant _aiConversant;
        private float rotationSpeed = 100f;
        private bool hasReachedDest = false;

        #region Basic Unity Methods

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _aiConversant = GetComponent<AIConversant>();
            _currentActionIndex = 0;
            _currentAction = npcActions[_currentActionIndex];
        }

        private void Update()
        {
            _timePassedInAction += Time.deltaTime;
            if (!hasReachedDest)
            {
                hasReachedDest = HasReachedActionDestination();
            }
            if (hasReachedDest)//HasReachedActionDestination() )
            {
                _agent.transform.rotation = Quaternion.RotateTowards(_agent.transform.rotation, _currentAction.destination.rotation, rotationSpeed * Time.deltaTime);
                if (!_inExecution)
                {
                    _timePassedInAction = 0;
                    ExecuteAction();
                }
            }

            if (_currentAction.timeInAction <= _timePassedInAction)
            {
                _timePassedInAction = 0;
                MoveToNextAction();
            }
        }

        #endregion

        #region Main Methods

        private void ExecuteAction()
        {
            _inExecution = true;
            
            _aiConversant.SetNewDialogue(_currentAction.dialogue);
        }

        private bool HasReachedActionDestination()
        {
            if (Vector3.Distance(_agent.transform.position, _currentAction.destination.position) < 2)
            {
                return true;
            }

            return false;
        }

        public void MoveToNextAction()
        {
            _inExecution = false;
            hasReachedDest = false;
            if (_currentActionIndex + 1 >= npcActions.Length)
            {
                return;
            }
            _currentActionIndex++;
            _currentAction = npcActions[_currentActionIndex];
            _aiConversant.SetNewDialogue(null);
            _mover.StartMoveAction(_currentAction.destination.position,_currentAction.speedToThisAction);
        }

        #endregion

        #region SavingSystem Methods


        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentActionIndex);
        }

        public void RestoreFromJToken(JToken state)
        {
            int index = state.ToObject<int>();
            _currentActionIndex = index;
            _currentAction = npcActions[index];
            _timePassedInAction = 0;
            
        }

        #endregion
    }
}
