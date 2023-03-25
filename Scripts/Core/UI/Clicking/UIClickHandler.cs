using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Core.UI.Clicking
{
    public class UIClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public event Action onLeftClick;
        public event Action onRightClick;
        public event Action onMiddleClick;

        #region Unity Event Methods

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onLeftClick?.Invoke();
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
            {
                onMiddleClick?.Invoke();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                onRightClick?.Invoke();
            }

        }

        #endregion
        
    }
}
