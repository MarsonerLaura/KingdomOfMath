using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Traits
{

    public class TraitRowUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueField;
        [SerializeField] private Button minusButton;
        [SerializeField] private Button plusButton;
        [SerializeField] private Trait trait;
        
        private TraitStore _traitStore = null;

        #region Basic Unity Methods

        private void Start()
        {
            _traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            minusButton.onClick.AddListener(()=> Allocate(-1));
            plusButton.onClick.AddListener(()=> Allocate(1));
        }

        private void Update()
        {
            minusButton.interactable = _traitStore.CanAssignPOints(trait, -1);
            plusButton.interactable = _traitStore.CanAssignPOints(trait, 1);
            
            valueField.text = _traitStore.GetProposedPoints(trait).ToString();
        }

        #endregion

        #region Main Methods

        public void Allocate(int points)
        {
            _traitStore.AssignPoints(trait, points);
        }

        #endregion
        
    }
}