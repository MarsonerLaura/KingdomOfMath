using System;
using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI balanceField;

        private Purse _playerPurse = null;
        
        #region Basic Unity Methods

        private void Awake()
        {
            _playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();
        }

        private void OnEnable()
        {
            if (_playerPurse != null)
            {
                _playerPurse.OnChange += RefreshUI;
            }
        }

        private void Start()
        {
            RefreshUI();
        }

        private void OnDisable()
        {
            if (_playerPurse != null)
            {
                _playerPurse.OnChange -= RefreshUI;
            }
        }

        #endregion

        #region Main Methods

        public void RefreshUI()
        {
            balanceField.text = $"{_playerPurse.GetBalance():N2}";
        }

        #endregion
        
    }

}