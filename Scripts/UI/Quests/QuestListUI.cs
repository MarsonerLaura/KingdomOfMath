using RPG.Quests;
using RPG.Saving;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] private QuestItemUI questItemPrefab;
        private QuestList _questList;

        #region Basic Unity Methods
        
        private void OnEnable()
        {
            _questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            _questList.OnUpdateQuestList += Redraw;
        }

        private void Start()
        {
            Redraw();
        }

        #endregion

        #region Main Methods

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            foreach (QuestStatus status in _questList.GetStatuses())
            {
                QuestItemUI uiInstance = Instantiate<QuestItemUI>(questItemPrefab, transform);
                uiInstance.Setup(status);
            }
        }

        #endregion
       
        
    }
}
