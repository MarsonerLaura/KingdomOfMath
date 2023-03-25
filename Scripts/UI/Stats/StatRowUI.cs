using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI.Stats
{

    public class StatRowUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueField;
        [SerializeField] private Stat stat;

        private BaseStats _baseStats = null;

        #region Basic Unity Methods

        private void Start()
        {
            _baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            valueField.text = _baseStats.GetStat(stat).ToString();
        }

        #endregion

    }
}
