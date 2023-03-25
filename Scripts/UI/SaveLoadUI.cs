using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    
    public class SaveLoadUI : MonoBehaviour
    {
        [SerializeField] private Transform contentTransform;
        [SerializeField] private GameObject buttonPrefab;

        #region Basic Unity Methods

        private void OnEnable()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if (savingWrapper == null)
            {
                return;
            }

            foreach (Transform child in contentTransform)
            {
                Destroy(child.gameObject);
            }

            foreach (string listSave in savingWrapper.ListSaves())
            {
                GameObject buttonInstance = Instantiate(buttonPrefab, contentTransform);
                buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = listSave;
                buttonInstance.GetComponentInChildren<Button>().onClick.AddListener(()=>
                {
                    savingWrapper.LoadGame(listSave);
                });
            }
        }

        #endregion
       
    }
}
