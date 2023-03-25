using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI questTitle;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Transform objectiveContainer;
        [SerializeField] private GameObject objectivePrefab;
        [SerializeField] private GameObject completedObjectivePrefab;

        #region Main Methods

        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            questTitle.text = quest.GetTitle();
            foreach (Transform item in objectiveContainer)
            {
                Destroy(item.gameObject);
            }
            foreach (var objective in quest.GetObjectives())
            {
                GameObject prefab = objectivePrefab;
                if (status.IsObjectiveComplete(objective.reference))
                {
                    prefab = completedObjectivePrefab;
                }
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
            }
            rewardText.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest quest)
        {
            string text = "";
            foreach (var reward in quest.GetRewards())
            {
                if (!text.Equals(""))
                {
                    text += ", ";
                }

                if (reward.number > 1)
                {
                    text += reward.number + " ";
                }
                text += reward.item.GetDisplayName();
            }
            if (text.Equals(""))
            {
                text = "No reward";
            }
            text += ".";
            return text;
        }

        #endregion
        
    }
}