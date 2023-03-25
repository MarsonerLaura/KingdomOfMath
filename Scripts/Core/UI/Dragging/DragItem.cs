using RPG.Inventories;
using RPG.UI.Inventories;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Core.UI.Dragging
{
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        where T : class
    {
        Vector3 startPosition;
        Transform originalParent;
        IDragSource<T> source;
        Canvas parentCanvas;

        #region Basic Unity Methods

        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            source = GetComponentInParent<IDragSource<T>>();
        }

        #endregion

        #region Main Methods

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            startPosition = transform.position;
            originalParent = transform.parent;
            // Else won't get the drop event.
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(parentCanvas.transform, true);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            transform.position = startPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(originalParent, true);

            IDragDestination<T> container;
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                container = parentCanvas.GetComponent<IDragDestination<T>>();
            }
            else
            {
                container = GetContainer(eventData);
            }

            if (container != null)
            {
                DropItemIntoContainer(container);
            }


        }

        private IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();

                return container;
            }
            return null;
        }

        private void DropItemIntoContainer(IDragDestination<T> destination)
        {
            if (object.ReferenceEquals(destination, source)) return;

            var destinationContainer = destination as IDragContainer<T>;
            var sourceContainer = source as IDragContainer<T>;

            // Swap won't be possible
            if (destinationContainer == null || sourceContainer == null ||
                destinationContainer.GetItem() == null||object.ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
            {
                AttemptSimpleTransfer(destination);
                return;
            }

            AttemptSwap(destinationContainer, sourceContainer);
        }
        

        private void AttemptSwap(IDragContainer<T> destination, IDragContainer<T> source)
        {
            // Provisionally remove item from both sides. 
            var removedSourceNumber = source.GetNumber();
            var removedSourceItem = source.GetItem();
            var removedDestinationNumber = destination.GetNumber();
            var removedDestinationItem = destination.GetItem();
            
            if (destination is RPG.UI.Inventories.MathSlotUI && source is RPG.UI.Inventories.MathSlotUI)
            {
                source.RemoveItems(removedSourceNumber);
                destination.RemoveItems(removedDestinationNumber);
                if (destination.MaxAcceptable(removedSourceItem) >= removedSourceNumber)
                {
                    destination.AddItems(removedSourceItem, removedSourceNumber);
                }
                else
                {
                    if (removedDestinationNumber > 0)
                    {
                        destination.AddItems(removedDestinationItem, removedDestinationNumber);
                    }
                    if (removedSourceNumber > 0)
                    {
                        source.AddItems(removedSourceItem, removedSourceNumber);
                    }
                    return;
                }
                if (source.MaxAcceptable(removedDestinationItem)>=removedDestinationNumber)
                {
                    source.AddItems(removedDestinationItem, removedDestinationNumber);
                }
                else
                {
                    if (removedDestinationNumber > 0)
                    {
                        destination.AddItems(removedDestinationItem, removedDestinationNumber);
                    }
                    if (removedSourceNumber > 0)
                    {
                        source.AddItems(removedSourceItem, removedSourceNumber);
                    }
                }
                return;
            }
            
            source.RemoveItems(removedSourceNumber);
            destination.RemoveItems(removedDestinationNumber);

            var sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, source, destination);
            var destinationTakeBackNumber = CalculateTakeBack(removedDestinationItem, removedDestinationNumber, destination, source);

            // Do take backs (if needed)
            if (sourceTakeBackNumber > 0)
            {
                source.AddItems(removedSourceItem, sourceTakeBackNumber);
                removedSourceNumber -= sourceTakeBackNumber;
            }
            if (destinationTakeBackNumber > 0)
            {
                destination.AddItems(removedDestinationItem, destinationTakeBackNumber);
                removedDestinationNumber -= destinationTakeBackNumber;
            }

            // Abort if we can't do a successful swap
            if (source.MaxAcceptable(removedDestinationItem) < removedDestinationNumber ||
                destination.MaxAcceptable(removedSourceItem) < removedSourceNumber ||
                removedSourceNumber == 0 || removedDestinationNumber==0)
            {
                if (removedDestinationNumber > 0)
                {
                    destination.AddItems(removedDestinationItem, removedDestinationNumber);
                }
                if (removedSourceNumber > 0)
                {
                    source.AddItems(removedSourceItem, removedSourceNumber);
                }
                return;
            }
            

            // Do swaps
            if (removedDestinationNumber > 0)
            {
                source.AddItems(removedDestinationItem, removedDestinationNumber);
            }
            if (removedSourceNumber > 0)
            {
                destination.AddItems(removedSourceItem, removedSourceNumber);
            }
        }

        private bool AttemptSimpleTransfer(IDragDestination<T> destination)
        {
            var draggingItem = source.GetItem();
            var draggingNumber = source.GetNumber();
            var acceptable = destination.MaxAcceptable(draggingItem);
            var toTransfer = Mathf.Min(acceptable, draggingNumber);
            var srcContainer = source as IDragContainer<T>;
            if (destination is RPG.UI.Inventories.MathSlotUI)
            {
                toTransfer = Mathf.Min(1, draggingNumber);
                source.RemoveItems(1);
                
                acceptable = destination.MaxAcceptable(draggingItem);
                toTransfer = Mathf.Min(acceptable, draggingNumber);
                if (toTransfer > 0)
                {
                    destination.AddItems(draggingItem, 1);
                    return false;
                }
                srcContainer.AddItems(draggingItem,1);
                return true;
            }
            if (source is RPG.UI.Inventories.MathSlotUI)
            {
                MathSlotUI slotUI = source as MathSlotUI;
                if (slotUI.IsStillValidValueAfterRemove())
                {
                    source.RemoveItems(1);
                    destination.AddItems(draggingItem, 1);
                    return false;
                }

                return true;

            }
            if (toTransfer > 0)
            {
                source.RemoveItems(toTransfer);
                destination.AddItems(draggingItem, toTransfer);
                return false;
            }

            return true;
        }

        private int CalculateTakeBack(T removedItem, int removedNumber, IDragContainer<T> removeSource, IDragContainer<T> destination)
        {
            var takeBackNumber = 0;
            var destinationMaxAcceptable = destination.MaxAcceptable(removedItem);

            if (destinationMaxAcceptable < removedNumber)
            {
                takeBackNumber = removedNumber - destinationMaxAcceptable;

                var sourceTakeBackAcceptable = removeSource.MaxAcceptable(removedItem);

                // Abort and reset
                if (sourceTakeBackAcceptable < takeBackNumber)
                {
                    return 0;
                }
            }
            return takeBackNumber;
        }

        #endregion

    }
}