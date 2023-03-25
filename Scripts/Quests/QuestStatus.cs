
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPG.Quests
{
    [System.Serializable]
    public class QuestStatus 
    {
        [SerializeField] private Quest quest;
        [SerializeField] private List<string> completedObjectives = new List<string>();

        [System.Serializable]
        class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }
        
        #region Main Methods
        
        public void CompleteObjective(string objective)
        {
            if (!quest.HasObjective(objective))
            {
                return;
            }
            completedObjectives.Add(objective);
        }
        
        public bool IsComplete()
        {
            foreach (var objective in quest.GetObjectives())
            {
                if (!completedObjectives.Contains(objective.reference))
                {
                    return false;
                }
            }
            return true;
        }
        
        #endregion

        #region Getters and Setters

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }
        #endregion

        #region Saving System Methods
        
        public QuestStatus(JToken objectState)
        {
            var state = objectState.ToObject<QuestStatusRecord>();
            quest = Quest.GetByName(state.questName);
            completedObjectives = state.completedObjectives;
        }
        
        public JToken CaptureAsJToken()
        {
            QuestStatusRecord state = new QuestStatusRecord();
            state.questName = quest.name;
            state.completedObjectives = completedObjectives;
            return JToken.FromObject(state);
        }
        
        #endregion

    }
}
