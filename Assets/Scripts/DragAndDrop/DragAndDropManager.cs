using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DragAndDropManager : MonoBehaviour
{
    [SerializeField] Transform dragHolder;

    static private bool _isDraging = false;

    private Slot _originalSlot;

    private bool _successDrop;

  //  private Slot _newSlot;

    private (Item item, GameObject go) _dragingObject;

    public event Action<Item> OnItemResponceAnimation = delegate { };

    // public (Item item, GameObject go) GetDragingObject { get { return _dragingObject; }}

    //public Transform GetOriginalLocation { get { return _originalLocation; } }
    //public Transform GetNewLocation { get { return _newLocation; } }

    static public bool IsDraging { get { return _isDraging; } set { _isDraging = value; } }

    #region Singleton
    private static DragAndDropManager _instantion;
    public static DragAndDropManager Instantion
    {
        get
        {
            if (_instantion == null)
            {
                GameObject go = new GameObject("DragAndDropManager");
                go.AddComponent<DragAndDropManager>();
            }

            return _instantion;
        }
    }
    private void Awake()
    {
        _instantion = this;
    }

    #endregion


    public void InitDraging((Item item, GameObject go) dragingObject)
    {
        _isDraging = true;
        _successDrop = false;
        _originalSlot = dragingObject.item.MySlot;
        _dragingObject = dragingObject;

        _dragingObject.go.transform.SetParent(dragHolder);

        var rect = dragingObject.go.GetComponent<RectTransform>();
        if (rect != null)
            rect.sizeDelta = new Vector2 (60,60);

        OnItemResponceAnimation.Invoke(dragingObject.item);

    }

    public void wasSuccessfullyDroped()
    {
        if (!_successDrop)
            ReturnToOriginSlot(_originalSlot);
    }

    public void SetDefault()
    {
        OnItemResponceAnimation.Invoke(_dragingObject.item);

        _dragingObject = (null, null);
        _originalSlot = null;
        _successDrop = false;
        _isDraging = false;
    }

    public void HandleDrop(Slot destination)
    {
        if (_dragingObject == (null, null)) return;

        if (destination == null)
        {
            // ToDo
            Debug.LogWarning("nekde je chyba :->  vrat item na puvodni pozici.. DRAG and DROP Manager");
            _successDrop = false;
        }

        if (destination is SpecInventorySlot specSlotDestination && _originalSlot is SpecInventorySlot specSlotOrigin)
        {
            ChangeItemSlot(specSlotDestination, specSlotOrigin);
        }
        else if (destination is SpecInventorySlot specSlotDestination2 && _originalSlot is ItemSlot itemSlotOrigin2)
        {
            ChangeItemSlot(specSlotDestination2, itemSlotOrigin2);
        }
        else if (destination is ItemSlot slotDestination && _originalSlot is ItemSlot slotOrigin)
        {
            ChangeItemSlot(slotDestination, slotOrigin); // budu potrebovat i druhy item ale možna me bude stacit ten co draguji... ??
        }
        else if (destination is ItemSlot itemSlotDestination && _originalSlot is SpecInventorySlot specSlotOrigin2)
        {
            ChangeItemSlot(itemSlotDestination, specSlotOrigin2);
        }

        _successDrop = true;
    }
    private void ChangeItemSlot(ItemSlot itemSlotDestination, SpecInventorySlot specSlotOrigin2)
    {
        if (itemSlotDestination.IsEmpty)
        {
            // prirad do volne pozice
            _dragingObject.go.transform.SetParent(itemSlotDestination.GetItemContainer);
            _dragingObject.item.MySlot = itemSlotDestination;
            itemSlotDestination.CurrentItem = _dragingObject;

            // original pozice
            specSlotOrigin2.CurrentItem = (null, null);

            itemSlotDestination.IsEmpty = false;
            specSlotOrigin2.IsEmpty = true;

        }
        else
        {
            if (itemSlotDestination.CurrentItem.item.Type != specSlotOrigin2.CurrentItem.item.Type)
            {
                ReturnToOriginSlot(specSlotOrigin2);
            }
            else
                SwitchItems(itemSlotDestination, specSlotOrigin2);
        }
    }
    private void ChangeItemSlot(ItemSlot slotDestination, ItemSlot slotOrigin)
    {
        if (slotDestination.IsEmpty)
        {
            // prirad do volne pozice
            _dragingObject.go.transform.SetParent(slotDestination.GetItemContainer);
            _dragingObject.item.MySlot = slotDestination;
            slotDestination.CurrentItem = _dragingObject;
            
            // original pozice
            slotOrigin.CurrentItem = (null, null);

            slotDestination.IsEmpty = false;
            slotOrigin.IsEmpty = true;
        }
        else
        {
            // switching 

            SwitchItems(slotDestination, slotOrigin);

        }
    }
    private void ChangeItemSlot(SpecInventorySlot specSlotDestination2, Slot itemSlotOrigin2)
    {
        if (specSlotDestination2.GetSlotType != itemSlotOrigin2.CurrentItem.item.Type)
        {
            // vrat item na puvodni pozici...
            ReturnToOriginSlot(itemSlotOrigin2);
        }
        else
        {
            if (specSlotDestination2.IsEmpty)
            {
                // prirad do volne pozice
                _dragingObject.go.transform.SetParent(specSlotDestination2.GetItemContainer);
                _dragingObject.item.MySlot = specSlotDestination2;
                specSlotDestination2.CurrentItem = _dragingObject;

                // original pozice
                itemSlotOrigin2.CurrentItem = (null, null);

                specSlotDestination2.IsEmpty = false;
                itemSlotOrigin2.IsEmpty = true;
            }
            else
            {
                // switching
                specSlotDestination2.CurrentItem.go.transform.SetParent(itemSlotOrigin2.GetItemContainer);
                itemSlotOrigin2.CurrentItem = specSlotDestination2.CurrentItem;
                itemSlotOrigin2.CurrentItem.item.MySlot = _dragingObject.item.MySlot;


                _dragingObject.go.transform.SetParent(specSlotDestination2.GetItemContainer);
                _dragingObject.item.MySlot = specSlotDestination2;
                specSlotDestination2.CurrentItem = _dragingObject;
            }
        }
    }
    private void SwitchItems(ItemSlot itemSlotDestination, SpecInventorySlot specSlotOrigin2)
    {
        itemSlotDestination.CurrentItem.go.transform.SetParent(specSlotOrigin2.GetItemContainer);
        specSlotOrigin2.CurrentItem = itemSlotDestination.CurrentItem;
        specSlotOrigin2.CurrentItem.item.MySlot = _dragingObject.item.MySlot;


        _dragingObject.go.transform.SetParent(itemSlotDestination.GetItemContainer);
        _dragingObject.item.MySlot = itemSlotDestination;
        itemSlotDestination.CurrentItem = _dragingObject;
    }
    private void SwitchItems(ItemSlot slotDestination, ItemSlot slotOrigin)
    {
        slotDestination.CurrentItem.go.transform.SetParent(slotOrigin.GetItemContainer);
        slotOrigin.CurrentItem = slotDestination.CurrentItem;
        slotOrigin.CurrentItem.item.MySlot = _dragingObject.item.MySlot;


        _dragingObject.go.transform.SetParent(slotDestination.GetItemContainer);
        _dragingObject.item.MySlot = slotDestination;
        slotDestination.CurrentItem = _dragingObject;
    }
    private void ReturnToOriginSlot(Slot specSlotOrigin2)
    {
        _dragingObject.go.transform.SetParent(specSlotOrigin2.GetItemContainer);
    }

}
