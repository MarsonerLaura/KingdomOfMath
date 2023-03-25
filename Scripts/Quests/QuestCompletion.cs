using UnityEngine;
using RPG.Inventories;
namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest quest;
        [SerializeField] private string objective;
        [SerializeField] private string itemId;
        [SerializeField] private int number;

        #region Main Methods

        public void CompleteObjective()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.CompleteObjective(quest, objective);
        }

        public void RemoveQuestItemsFromInventory()
        {
            Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
            inventory.RemoveQuestItems(itemId, number);
        }

        #endregion
        
    }
}