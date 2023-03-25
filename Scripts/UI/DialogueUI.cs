using System;
using RPG.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI aiText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Transform choiceRootTransform;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private GameObject aiResponse;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI conversantName;
        
        private PlayerConversant _playerConversant;

        #region Basic Unity Methods

        private void Awake()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        }

        private void OnEnable()
        {
            _playerConversant.OnConversationUpdated += UpdateUI;
        }

        private void Start()
        {
            nextButton.onClick.AddListener(() => _playerConversant.Next());
            quitButton.onClick.AddListener(() => _playerConversant.Quit());
            UpdateUI();
        }

        #endregion

        #region Main Methods

        private void UpdateUI()
        {
            gameObject.SetActive(_playerConversant.IsActive());
            if (!_playerConversant.IsActive())
            {
                return;
            }

            conversantName.text = _playerConversant.GetCurrentConversantName();
            aiResponse.SetActive(!_playerConversant.IsChoosing());
            choiceRootTransform.gameObject.SetActive(_playerConversant.IsChoosing());
            
            if (_playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                aiText.text = _playerConversant.GetText();
                nextButton.gameObject.SetActive(_playerConversant.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform choice in choiceRootTransform)
            {
                Destroy(choice.gameObject);
            }

            foreach (DialogueNode choiceNode in _playerConversant.GetChoices())
            {
                GameObject currentChoice = Instantiate(choicePrefab, choiceRootTransform);
                var choice = currentChoice.GetComponentInChildren<TextMeshProUGUI>();
                choice.text = choiceNode.GetText();
                Button button = currentChoice.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    _playerConversant.SelectChoice(choiceNode);
                });
            }
        }

        #endregion
        
    }
}