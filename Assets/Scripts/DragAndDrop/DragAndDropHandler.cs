using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropHandler : MonoBehaviour, IPointerHandler, IDragable , IDropHandler
{
    private CanvasGroup canvasGroup;
 
    private ( Item item, GameObject go) itemInSlot;

    [SerializeField] public bool _disableDrag = false;

    private bool _beginDrag = false;

    public bool GetBeginDrag { get { return this._beginDrag; } }

    public void InitDragHandler()
    {
        itemInSlot.go = this.gameObject;
        itemInSlot.item = this.gameObject.GetComponent<Item>();
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_disableDrag && !DragAndDropManager.IsDraging)
        {
            _beginDrag = true;
            MakeTransparent(true);
            DragAndDropManager.Instantion.InitDraging(itemInSlot);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (!_disableDrag && _beginDrag)
        {
            MakeTransparent(false);

            if(DragAndDropManager.IsDraggingProceed)
            {
                DragAndDropManager.IsDraging = true;
                DragAndDropManager.Instantion.WasSuccessfullyDroped();
            }
            else
            {
                DragAndDropManager.IsDraging = false;

                DragAndDropManager.Instantion.WasSuccessfullyDroped();

                DragAndDropManager.Instantion.SetDefault();
            }

            _beginDrag = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        bool result = false;

        if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Middle)
        {
            if (DragAndDropManager.IsDraging)
            {
                result = DragAndDropManager.Instantion.HandleDrop(itemInSlot.item.MySlot);
            }
            else
            {
                if (!_disableDrag)
                {
                    MakeTransparent(true);
                    DragAndDropManager.Instantion.InitDraging(itemInSlot);
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Item Action if has ofc.
            if (itemInSlot.item.MySlot is SpecInventorySlot specSlot)
            {
                // for backpack
                if(specSlot.HasSlotThatType(ItemBlueprint.ItemType.BagSpec))
                {
                    Backpack backpack = itemInSlot.item as Backpack;
                    specSlot.OpenBackPack(backpack.Capacity);
                }
            }
        }

        DragAndDropManager.Instantion.TryEndDrag();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_disableDrag)
        {
            DragAndDropManager.Instantion.HandleDrop(itemInSlot.item.MySlot);
        }

        DragAndDropManager.Instantion.TryEndDrag();
    }

    public void MakeTransparent(bool transparent)
    {
        if(transparent)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.7f;
        }
        else
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
    }

    public void OnEndDrag()
    {
        if (!_disableDrag)
        {
            MakeTransparent(false);

            if (DragAndDropManager.IsDraggingProceed)
            {
                DragAndDropManager.IsDraging = true;
                DragAndDropManager.Instantion.WasSuccessfullyDroped();
            }
            else
            {
                DragAndDropManager.IsDraging = false;

                DragAndDropManager.Instantion.WasSuccessfullyDroped();

                DragAndDropManager.Instantion.SetDefault();
            }
        }
    }
}
