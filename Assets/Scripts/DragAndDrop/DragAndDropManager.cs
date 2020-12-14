using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropManager : MonoBehaviour
{
    [SerializeField] Transform dragHolder;

    static private bool _isDraging = false;

    private Slot _originalSlot;

    private bool _successDrop;

  //  private Slot _newSlot;

    private (Item item, GameObject go) _dragingObject;

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

    }

    public void WasItemReplaced()
    {
        if(!_successDrop)
        {
            _dragingObject.go.transform.SetParent(_originalSlot.GetItemContainer);
        }
    }

    //public bool IsDropItemPosible()
    //{
    //    if (_newSlot == null || _newSlot == _originalSlot)
    //        return false;

    //    if (_originalSlot == null) // Todo tohle by tak nemelo byt..
    //        return false;

    //    return true;
    //}

    //public bool ReturnToOriginalPosition()
    //{
    //    if (_newSlot == null || _newSlot == _originalSlot)
    //    {
    //        MoveItem(_dragingObject, _dragingObject.item.MySlot.GetItemContainer);
    //        SetDefault();
    //    }

    //    return false;
    //}

    //public void SetDropPosition(Slot destinationSlot)
    //{
    //    if(_originalSlot is SpecInventorySlot specSlot)
    //    {
    //        if(specSlot.GetSlotType != _dragingObject.item.blueprint.Type)
    //        {
    //            return;
    //        }
    //    }

    //    if (destinationSlot is ItemSlot itemSlot && _originalSlot is SpecInventorySlot origin)
    //    {
    //        if (!itemSlot.IsEmpty && origin.GetSlotType == itemSlot.CurrentItem.item.Blueprint.Type)
    //        {
    //            _newSlot = destinationSlot;
    //        }
    //        else if (itemSlot.IsEmpty)
    //        {
    //            _newSlot = destinationSlot;
    //        }

    //        return;

    //    }


    //    _newSlot = destinationSlot;
    //}

    //public bool SwitchPosition()
    //{

    //    if (_dragingObject.go != null || _newSlot != null)
    //    {
    //        if (!_newSlot.IsEmpty) // Switchuji Itemy.
    //        {
    //            MoveItem(_newSlot.CurrentItem, _originalSlot.GetItemContainer);

    //            _newSlot.CurrentItem.item.MySlot = _originalSlot;

    //            _originalSlot.CurrentItem = _newSlot.CurrentItem;
    //            _newSlot.CurrentItem = _dragingObject;

    //        }
    //        else // Je prazdny slot prideluji
    //        {
    //            _newSlot.IsEmpty = false;
    //            _originalSlot.IsEmpty = true;

    //            _newSlot.CurrentItem = _dragingObject;
    //            _originalSlot.CurrentItem = (null, null);
    //        }

    //        MoveItem(_dragingObject, _newSlot.GetItemContainer);
    //        _dragingObject.item.MySlot = _newSlot;
    //        SetDefault();

    //        return true;
    //    }


    //    return false;
    //}

    public void SetDefault()
    {
        _dragingObject = (null, null);
        _originalSlot = null;
        _successDrop = false;
        _isDraging = false;
    }


    ///////////////////////////////////////////////////////////////////////
    /// Druhy pokus ........

    public void HandleDrop(Slot destination)
    {

        Debug.Log(_dragingObject + "  _draging object");
        Debug.Log(_successDrop + "  _successDrop");
        Debug.Log(_originalSlot + "  _originalSlot");
        Debug.Log(_isDraging + "  _isDraging");

        if (_dragingObject == (null, null)) return;

        if (destination == null)
        {
            // ToDo
            Debug.Log("nekde je chyba :->  vrat item na puvodni pozici..");
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
            if (itemSlotDestination.CurrentItem.item.GetType != specSlotOrigin2.CurrentItem.item.GetType)
            {
                // vrat item na puvodni pozici...
            }
            else
            {
                // switch
            }
        }
    }

    private void ChangeItemSlot(ItemSlot slotDestination, ItemSlot slotOrigin)
    {
        if (slotDestination.IsEmpty)
        {
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
            // vymena 

            slotDestination.CurrentItem.go.transform.SetParent(slotOrigin.GetItemContainer);
            slotOrigin.CurrentItem = slotDestination.CurrentItem;
            slotOrigin.CurrentItem.item.MySlot = _dragingObject.item.MySlot;


            _dragingObject.go.transform.SetParent(slotDestination.GetItemContainer);
            _dragingObject.item.MySlot = slotDestination;
            slotDestination.CurrentItem = _dragingObject;

        }
    }

    private void ChangeItemSlot(SpecInventorySlot specSlotDestination2, Slot itemSlotOrigin2)
    {
        if (specSlotDestination2.GetSlotType != itemSlotOrigin2.CurrentItem.item.GetType)
        {
            // vrat item na puvodni pozici...
        }
        else
        {
            if (specSlotDestination2.IsEmpty)
            {
                // prirad do volne pozice
            }
            else
            {
                // vymena
            }
        }
    }
}
