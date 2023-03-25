using System;
using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI progress;
        private QuestStatus _questStatus;

        #region Main Methods

        public void Setup(QuestStatus questStatus)
        {
            _questStatus = questStatus;
            title.text = questStatus.GetQuest().GetTitle();
            progress.text = String.Concat(questStatus.GetCompletedCount(),"/",questStatus.GetQuest().GetObjectiveCount());
        }

        #endregion

        #region Getters and Setters

        public QuestStatus GetQuestStatus()
        {
            return _questStatus;
        }

        #endregion
        
    }
}
