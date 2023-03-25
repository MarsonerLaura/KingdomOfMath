using RPG.Inventories;
using UnityEngine;
using TMPro;


namespace RPG.UI.Inventories
{

    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI titleText = null;
        [SerializeField] TextMeshProUGUI bodyText = null;

        #region Main Methods

        public void Setup(InventoryItem item)
        {
            titleText.text = item.GetDisplayName();
            bodyText.text = item.GetDescription();
        }

        #endregion
        
    }
}
