using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Inventories;
using RPG.Saving;
using RPG.Utils;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, IJsonSaveable, IPredicateEvaluator
    {
        private List<QuestStatus> _statuses = new List<QuestStatus>();
        public event Action OnUpdateQuestList;

        #region Basic Unity Methods

        private void Update()
        {
            CompleteObjectivesByPredicates();
        }

        #endregion

        #region Main Methods
        
        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest))
            {
                return;
            }
            QuestStatus newStatus = new QuestStatus(quest);
            _statuses.Add(newStatus);
            OnUpdateQuestList?.Invoke();
        }

        private bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        } 
        
        private void CompleteObjectivesByPredicates()
        {
            foreach (QuestStatus status in _statuses)
            {
                if (status.IsComplete())
                {
                    continue;
                }

                Quest quest = status.GetQuest();
                foreach (var objective in quest.GetObjectives())
                {
                    if (status.IsObjectiveComplete(objective.reference))
                    {
                        continue;
                    }
                    if (!objective.usesCondition)
                    {
                        continue;
                    }

                    if (objective.completionCondition.Check(GetComponents<IPredicateEvaluator>()))
                    {
                        CompleteObjective(quest, objective.reference);
                    }
                }
            }
        }


        #endregion

        #region Getters and Setters

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return _statuses;
        }
        
        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if (status.IsComplete())
            {
                GiveReward(quest);
            }
            OnUpdateQuestList?.Invoke();
        }


        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.GetRewards())
            {
                if (!reward.item.IsStackable() && reward.number > 1)
                {
                    for (int i = 0; i < reward.number; i++)
                    {
                        ProcessReward(reward.item, 1);
                    }
                }
                else
                {
                    ProcessReward(reward.item, reward.number);
                }
            }
        }

        private void ProcessReward(InventoryItem reward, int number)
        {
            bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward, number);
            if (!success)
            {
                GetComponent<ItemDropper>().DropItem(reward, number);
            }
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus status in _statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }

            return null;
        }
        
        

        #endregion

        #region Saving System Methods

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken>  stateList = state;
            foreach (QuestStatus status in _statuses)
            {
                //We're adding a key of the instance ID just to ensure unique dictionary entries
                stateList.Add($"{status.GetQuest().GetInstanceID()}", status.CaptureAsJToken());
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            IDictionary<string, JToken> stateList = state.ToObject<JObject>();
            
            _statuses.Clear();
            foreach (KeyValuePair<string, JToken> objectState in stateList)
            {
                //We're not concerned with the key, just the value
                _statuses.Add(new QuestStatus(objectState.Value));
            }
            OnUpdateQuestList?.Invoke();
        }



        #endregion

        #region Evaluation Methods

        public bool? Evaluate(IPredicateEvaluator.PredicateType predicate, string[] parameters)
        {
            switch (predicate)
            {
                case IPredicateEvaluator.PredicateType.HasQuest:
                    return HasQuest(Quest.GetByName(parameters[0]));
                case IPredicateEvaluator.PredicateType.CompletedQuest:
                    if (GetQuestStatus(Quest.GetByName(parameters[0])) == null)
                    {
                        return false;
                    }
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();
                case IPredicateEvaluator.PredicateType.CompletedObjectiveOfQuest:
                    if (GetQuestStatus(Quest.GetByName(parameters[0])) == null)
                    {
                        return false;
                    }
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsObjectiveComplete(parameters[1]);
            }

            return null;
        }

        #endregion
        
    }
}