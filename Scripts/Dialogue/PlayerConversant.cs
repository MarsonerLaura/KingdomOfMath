using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Utils;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour, IAction
    {

        [SerializeField] private string playerName;
        private Dialogue _currentDialogue;
        private DialogueNode _currentNode = null;
        private bool _isChoosing = false;
        public event Action OnConversationUpdated;
        private AIConversant _currentConversant = null;
        private AIConversant _targetConversant;

        #region Basic Unity Methods

        private void Update()
        {
            if (_targetConversant)
            {
                if (Vector3.Distance(_targetConversant.transform.position, transform.position) > 3)
                {
                    GetComponent<Mover>().MoveTo(_targetConversant.transform.position, 1);
                }
                else
                {
                    GetComponent<Mover>().Cancel();
                    StartDialogue();
                }
            }
        }

        #endregion

        #region Main Methods
        public void StartDialogueAction(AIConversant newConversant, Dialogue newDialogue)
        {
            if (_currentConversant != null && _currentConversant == newConversant) return;
            if (_currentDialogue != null) Quit();
            if (newDialogue == null)
            {
                return;
            }

            GetComponent<ActionSheduler>().StartAction(this);
            _targetConversant = newConversant;
            _currentDialogue = newDialogue;
        }
        
        private void StartDialogue()
        {
            _currentConversant = _targetConversant;
            _targetConversant = null;
            _currentNode = _currentDialogue.GetRootNode();
            OnConversationUpdated?.Invoke();
        }

        public void Cancel()
        {
            _targetConversant = null;
        }

        public void Quit()
        {
            
            _currentDialogue = null;
            TriggerExitAction();
            _currentNode = null;
            _currentConversant = null;
            _isChoosing = false;
            OnConversationUpdated?.Invoke();
        }
        
        public bool IsActive()
        {
            return _currentDialogue != null;
        }
        
        public bool IsChoosing()
        {
            return _isChoosing;
        }
        
        public string GetText()
        {
            if (_currentNode == null)
            {
                return "";
            }

            return _currentNode.GetText();
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            TriggerEnterAction();
            _isChoosing = false;
            if (HasNext())
            {
                Next(); 
            }
            else
            {
                Quit();
            }
        }
        
        public void Next()
        {
            int numPlayerResponses = FilterOnCondition(_currentDialogue.GetPlayerChildren(_currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                _isChoosing = true;
                TriggerExitAction();
                OnConversationUpdated?.Invoke();
                return;
            }
            DialogueNode[] children = FilterOnCondition(_currentDialogue.GetAIChildren(_currentNode)).ToArray();
            int randomIndex = Random.Range(0, children.Length - 1);
            TriggerExitAction();
            _currentNode = children[randomIndex];
            TriggerEnterAction();
            OnConversationUpdated?.Invoke();
        }

        public bool HasNext()
        {
            return FilterOnCondition(_currentDialogue.GetAllChildren(_currentNode)).ToArray().Length > 0;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(_currentDialogue.GetPlayerChildren(_currentNode));
        }
        
        public string GetCurrentConversantName()
        {
            if (_isChoosing)
            {
                return playerName;
            }
            else
            {
                return _currentConversant.GetConversantName();
            }
        }


        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if (_currentNode == null)
            {
                return;
            }
            TriggerAction(_currentNode.GetOnEnterAction());
        }
        
        private void TriggerExitAction()
        {
            if (_currentNode == null)
            {
                return;
            }
            TriggerAction(_currentNode.GetOnExitAction());
        }

        private void TriggerAction(string action)
        {
            if (action == "")
            {
                return;
            }
            foreach (DialogueTrigger trigger in _currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        #endregion
        
    }
}