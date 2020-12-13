using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropManager : MonoBehaviour
{
    [SerializeField] Transform dragHolder;

    static private bool _isDraging = false;

    private Slot _originalSlot;

    private Slot _newSlot;

    private (Item item, GameObject go) _dragingObject;

    public (Item item, GameObject go) GetDragingObject { get { return _dragingObject; }}

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

    public void SetOriginSlot(Slot originalSlot)
    {
        _originalSlot = originalSlot;
    }

    public void InitDraging((Item item, GameObject go) dragingObject)
    {
        _isDraging = true;
        MoveItem(dragingObject, dragHolder);
        _dragingObject = dragingObject;
    }

    public bool IsDropItemPosible()
    {
        if (_newSlot == null || _newSlot == _originalSlot)
            return false;

        if (_originalSlot == null) // Todo tohle by tak nemelo byt..
            return false;

        return true;
    }

    public bool ReturnToOriginalPosition()
    {
        if (_newSlot == null || _newSlot == _originalSlot)
        {
            MoveItem(_dragingObject, _dragingObject.item.MySlot.GetItemContainer);
            SetDefault();
        }

        return false;
    }

    public void SetDropPosition(Slot destinationSlot)
    {
        if(_originalSlot is SpecInventorySlot specSlot)
        {
            if(specSlot.GetSlotType != _dragingObject.item.blueprint.Type)
            {
                return;
            }
        }

        if (destinationSlot is ItemSlot itemSlot && _originalSlot is SpecInventorySlot origin)
        {
            if (!itemSlot.IsEmpty && origin.GetSlotType == itemSlot.CurrentItem.item.Blueprint.Type)
            {
                _newSlot = destinationSlot;
            }
            else if (itemSlot.IsEmpty)
            {
                _newSlot = destinationSlot;
            }

            return;

        }


        _newSlot = destinationSlot;
    }

    public bool SwitchPosition()
    {

        if (_dragingObject.go != null || _newSlot != null)
        {
            if (!_newSlot.IsEmpty) // Switchuji Itemy.
            {
                MoveItem(_newSlot.CurrentItem, _originalSlot.GetItemContainer);

                _newSlot.CurrentItem.item.MySlot = _originalSlot;

                _originalSlot.CurrentItem = _newSlot.CurrentItem;
                _newSlot.CurrentItem = _dragingObject;

            }
            else // Je prazdny slot prideluji
            {
                _newSlot.IsEmpty = false;
                _originalSlot.IsEmpty = true;

                _newSlot.CurrentItem = _dragingObject;
                _originalSlot.CurrentItem = (null, null);
            }

            MoveItem(_dragingObject, _newSlot.GetItemContainer);
            _dragingObject.item.MySlot = _newSlot;
            SetDefault();

            return true;
        }


        return false;
    }

    private void SetDefault ()
    {
        _originalSlot = _newSlot;
        _newSlot = null;
        _dragingObject.go = null;
        _dragingObject.item = null;
    }


    private void MoveItem((Item item , GameObject go) item , Transform destination)
    {
        item.go.transform.SetParent(destination);
    }
}
