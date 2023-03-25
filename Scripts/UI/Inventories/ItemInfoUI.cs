using System;
using RPG.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.UI.Inventories
{
    public class ItemInfoUI : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private Image icon;
        
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private TextMeshProUGUI type;
        
        [SerializeField] private GameObject stat;
        [SerializeField] private GameObject statValue;
        [SerializeField] private GameObject damage;
        [SerializeField] private GameObject damageValue;
        [SerializeField] private GameObject range;
        [SerializeField] private GameObject rangeValue;
        
        [SerializeField] private TextMeshProUGUI description;
       
        [SerializeField] private TextMeshProUGUI statName;
        [SerializeField] private TextMeshProUGUI statPercentage;
        [SerializeField] private Slider slider;

        [SerializeField] private GameObject statAmplification;
        [SerializeField] private GameObject line;
        
        
        MathStore mathStore;
        private InventoryItem currentItem;

        #region Basic Unity Methods

        private void OnEnable()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            mathStore = player.GetComponent<MathStore>();
            mathStore.mathStoreUpdated += RedrawUI;
        }

        private void Start()
        {
            RedrawUI();
        }

        #endregion

        #region Main Methods

        private void RedrawUI()
        {
            if (mathStore == null)
            {
                return;
            }
            currentItem = mathStore.currentItem;
            if (!mathStore.IsValidItem())
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            
        
            
            name.text=currentItem.GetDisplayName();
            icon.sprite = currentItem.GetIcon();
            level.text = currentItem.GetLevel().ToString();
            type.text = currentItem.GetCategory().ToString();
            description.text = currentItem.GetDescription();
            
            if (!mathStore.HasStat())
            {
                SetStatUIState(false);
            }
            
            else
            {
                SetStatUIState(true);
                string mainStat = mathStore.GetStatLabel();
                float percentageValue = mathStore.GetPercentageValue();
                float currentValue = mathStore.GetCurrentValue();
                float maxValue = mathStore.GetMaxValue();

                if (currentItem is RPG.Combat.WeaponConfig)
                {
                    SetWeaponUIState(true);
                    damage.GetComponent<TextMeshProUGUI>().text = "Damage";
                    damageValue.GetComponent<TextMeshProUGUI>().text = mathStore.GetDamageValue();
                    range.GetComponent<TextMeshProUGUI>().text = "Range";
                    rangeValue.GetComponent<TextMeshProUGUI>().text = mathStore.GetRangeValue();
                }
                else
                {
                    SetWeaponUIState(false);
                }

                stat.GetComponent<TextMeshProUGUI>().text = "*"+mainStat;
                statValue.GetComponent<TextMeshProUGUI>().text = String.Format("{0:0.#}",currentValue) + "/"+String.Format("{0:0.#}",maxValue);

                statName.text = "Bonus"+mainStat;
            
                if (percentageValue == 0||maxValue==0)
                {
                    slider.value = 0;
                    statPercentage.text = 0 +"%";
                }
                else
                {
                    slider.value = percentageValue;
                    statPercentage.text = String.Format("{0:0.##}%",percentageValue );
                }
                
            }
            
            
        }

        private void SetStatUIState(bool isActive)
        {
            statAmplification.SetActive(isActive);
            line.SetActive(isActive);
            stat.SetActive(isActive);
            statValue.SetActive(isActive);
            SetWeaponUIState(isActive);
        }

        private void SetWeaponUIState(bool isActive)
        {
            damage.SetActive(isActive);
            damageValue.SetActive(isActive);
            range.SetActive(isActive);
            rangeValue.SetActive(isActive);
        }

        #endregion
        
    }
}
