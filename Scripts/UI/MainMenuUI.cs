using RPG.SceneManagement;
using RPG.Utils;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField saveFileInputField;
        
        private LazyValue<SavingWrapper> _savingWrapper;
        private string tutorialSaveFile = "Tutorial";

        #region Basic Unity Methods

        private void Awake()
        {
            _savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }

        #endregion
        
        #region Main Methods

        public void ContinueGame()
        {

            _savingWrapper.value.ContinueGame();
        }

        public void NewGame()
        {
            _savingWrapper.value.NewGame(saveFileInputField.text);
        }
        

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Settings()
        {
            //TODO
        }

        public void Tutorial()
        {
            _savingWrapper.value.Tutorial(tutorialSaveFile);
        }
        
        private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }

        #endregion
        
    }
}