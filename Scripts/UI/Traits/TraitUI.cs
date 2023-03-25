using System;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Traits
{
    public class TraitUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI unassignedPointsField;
        [SerializeField] private Button commitButton;
        
        private TraitStore _traitStore = null;

        #region Basic Unity Methods

        private void Start()
        {
            _traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            commitButton.onClick.AddListener(_traitStore.Commit);
        }

        private void Update()
        {
            unassignedPointsField.text = _traitStore.GetUnassignedPoints().ToString();
        }

        #endregion
       
    }
}