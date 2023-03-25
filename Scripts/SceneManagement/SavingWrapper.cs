using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float fadeInTime = 0.2f;
        [SerializeField] private float fadeOutTime = 0.2f;
        [SerializeField] private int firstSceneBuildIndex = 3;
        [SerializeField] private int tutorialSceneBuildIndex = 1;
        [SerializeField] private int menuBuildIndex = 0;
        
        private const string CurrentSaveFileName = "currentSaveFileName";
        
        private JsonSavingSystem _jsonSavingSystem;

        #region Basic Unity Methods

        private void Awake()
        {
            _jsonSavingSystem = GetComponent<JsonSavingSystem>();

        }

        #endregion
        
        #region Main Methods
        
        public void Save()
        {
            _jsonSavingSystem.Save(GetCurrentSaveFile());
        }
        
        public void Load()
        {
            _jsonSavingSystem.Load(GetCurrentSaveFile());
        }

        public void Delete()
        {
            _jsonSavingSystem.Delete(GetCurrentSaveFile());
        }
        
        private void SaveAction(InputAction.CallbackContext callbackContext)
        {
            Save();
        }
        
        private void LoadAction(InputAction.CallbackContext callbackContext)
        {
            Load();
        }
        
        private void DeleteAction(InputAction.CallbackContext callbackContext)
        {
            Delete();
        }

        private IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return GetComponent<JsonSavingSystem>().LoadLastScene(GetCurrentSaveFile());
            yield return fader.FadeIn(fadeInTime);
        }

        public void ContinueGame()
        {
            if (!PlayerPrefs.HasKey(CurrentSaveFileName))
            {
                //maybe disable continue button
                return;
            }

            if (!_jsonSavingSystem.SaveFileExists(GetCurrentSaveFile()))
            {
                return;
            }

            if (GetCurrentSaveFile().Equals("Tutorial")){
                return;
            }
            
            StartCoroutine(LoadLastScene());
        }

        public void LoadGame(string saveFile)
        {
            SetCurrentSaveFile(saveFile);
            ContinueGame();
        }
        public void NewGame(string saveFile)
        {
            if (String.IsNullOrEmpty(saveFile))
            {
                return;
            }
            SetCurrentSaveFile(saveFile);
            StartCoroutine(LoadFirstScene());
        }

        public void Tutorial(string saveFile)
        {
            if (String.IsNullOrEmpty(saveFile))
            {
                return;
            }
            SetCurrentSaveFile(saveFile);
            
            _jsonSavingSystem.Delete(GetCurrentSaveFile());
            
            
            StartCoroutine(LoadTutorial());
        }

        private void SetCurrentSaveFile(string saveFile)
        {
            
            PlayerPrefs.SetString(CurrentSaveFileName, saveFile);
        }

        private string GetCurrentSaveFile()
        {
            return PlayerPrefs.GetString(CurrentSaveFileName);
        }
        
        private IEnumerator LoadFirstScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(firstSceneBuildIndex);
            yield return fader.FadeIn(fadeInTime);
        }

        private IEnumerator LoadTutorial()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(tutorialSceneBuildIndex);
            yield return fader.FadeIn(fadeInTime);
        }

        public IEnumerable<string> ListSaves()
        {
            return _jsonSavingSystem.ListSaves();
        }
        
        private IEnumerator LoadMenuScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(menuBuildIndex);
            yield return fader.FadeIn(fadeInTime);
        }
        
        public void LoadMenu()
        {
            StartCoroutine(LoadMenuScene());
        }
        
        #endregion
        
    }
}
