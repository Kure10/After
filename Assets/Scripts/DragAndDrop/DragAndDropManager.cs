using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DragAndDropManager : MonoBehaviour
{

    [SerializeField] Inventory inventory;

    [SerializeField] Transform dragHolder;

    [SerializeField] Transform dragBackpackHolder;

    static private bool _isDraging = false;

    static private bool _draggingProceed = false;

    private Slot _originalSlot;

    private bool _successDrop;

    private (Item item, GameObject go) _dragingObject;

    public event Action<Item> OnItemResponseReaction = delegate { };

    //
    List<(Item item, GameObject go)> dragingBackpackItems = new List<(Item item, GameObject go)>();
    //

    static public bool IsDraging { get { return _isDraging; } set { _isDraging = value; } }

    static public bool IsDraggingProceed { get { return _draggingProceed; } set { _draggingProceed = value; } }

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
        _originalSlot = dragingObject.item.MySlot; // nepotrebuji..
        _dragingObject = dragingObject;

        _dragingObject.go.transform.SetParent(dragHolder);

        var rect = dragingObject.go.GetComponent<RectTransform>();
        if (rect != null)
            rect.sizeDelta = new Vector2(60, 60);

        if(dragingObject.item.MySlot is SpecInventorySlot specInventorySlot && _dragingObject.item is Backpack backpakc)
        {
            specInventorySlot.CloseBackpack();
            dragingBackpackItems.Clear();

            List<SpecInventorySlot> charBackpackSlots = specInventorySlot.GetSpecialist.GetCharacterBackpackSlots();

            foreach (SpecInventorySlot slot in charBackpackSlots)
            {
                // set active false staci...
                if(slot.CurrentItem != (null, null))
                    slot.CurrentItem.go.transform.SetParent(dragBackpackHolder);

                dragingBackpackItems.Add(slot.CurrentItem);
                slot.CurrentItem = (null, null);
                slot.IsEmpty = true;
            }

            //if (originBackpackSlot != specSlotDestination2)
            //{
            //    originBackpackSlot.CloseBackpack();
            //}  

            // tmp vypis
            foreach ((Item item, GameObject go) item in dragingBackpackItems)
            {
                if (item != (null, null))
                    Debug.Log("item: " + item.item.Name);
            }
        }

        OnItemResponseReaction.Invoke(dragingObject.item);
    }

    public bool IsThisDragingItem((Item item, GameObject go) item)
    {
        if (_dragingObject == item)
            return true;

        return false;
    }

    private void Update()
    {
        if (IsDraging && _dragingObject != (null,null))
        {
            Vector3 posMouse = Input.mousePosition;
            _dragingObject.go.transform.position = posMouse;
            //rect.anchoredPosition += eventData.delta / canvas.scaleFactor;

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ReturnToOriginSlot();
                SetDefault();
            }
        }
    }

    public void wasSuccessfullyDroped()
    {
        if (!_successDrop)
            ReturnToOriginSlot(_originalSlot);
    }

    public void SetDefault()
    {
        if (_dragingObject.item is Backpack back && _draggingProceed)
        {
            ReturnToOriginSlot();
        }

        _isDraging = false;
        _draggingProceed = false;

        if(_dragingObject != (null, null) && _dragingObject.item.MySlot is SpecInventorySlot)
        {
            OnItemResponseReaction.Invoke(_dragingObject.item);
        }
        _dragingObject = (null, null);
        _originalSlot = null;
        _successDrop = false;
        OnItemResponseReaction.Invoke(_dragingObject.item);
    }

    public bool HandleDrop(Slot destination)
    {
        if (_dragingObject == (null, null)) return false;

        if (destination == null)
        {
            // ToDo
            Debug.LogWarning("nekde je chyba :->  vrat item na puvodni pozici.. DRAG and DROP Manager");
            _successDrop = false;
        }

        // pokud vracím do stejneho slotu
        if(destination == _originalSlot)
        {
            if(!IsDraggingProceed)
            {
                ReturnToOriginSlot();
                return _successDrop = true;
            }
        }

        if (destination is SpecInventorySlot specSlotDestination && _originalSlot is SpecInventorySlot specSlotOrigin)
        {
            if (_dragingObject.item is Backpack && specSlotDestination.IsBackpack)
                return false;

            _successDrop = ChangeItemSlot(specSlotDestination, specSlotOrigin);
        }
        else if (destination is SpecInventorySlot specSlotDestination2 && _originalSlot is ItemSlot itemSlotOrigin2)
        {
            if (_dragingObject.item is Backpack && specSlotDestination2.IsBackpack)
                return false;

            _successDrop = ChangeItemSlot(specSlotDestination2, itemSlotOrigin2);
        }
        else if (destination is ItemSlot slotDestination && _originalSlot is ItemSlot slotOrigin)
        {
            _successDrop = ChangeItemSlot(slotDestination, slotOrigin); // budu potrebovat i druhy item ale možna me bude stacit ten co draguji... ??
        }
        else if (destination is ItemSlot itemSlotDestination && _originalSlot is SpecInventorySlot specSlotOrigin2)
        {
            _successDrop = ChangeItemSlot(itemSlotDestination, specSlotOrigin2);
        }

        if (!_successDrop)
            SetDefault();

        return _successDrop;
    }
    private bool ChangeItemSlot(ItemSlot itemSlotDestination, SpecInventorySlot specSlotOrigin2)
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

            _originalSlot = null;
            IsDraggingProceed = false;

        }
        else
        {
                SwitchItems(itemSlotDestination, specSlotOrigin2);
        }

        if (_dragingObject.item is Backpack backpack)
        {
            foreach ((Item item, GameObject go) backpackItem in dragingBackpackItems)
            {
                if (backpackItem != (null, null))
                {
                    ItemSlot emptySlot = inventory.FindEmptySlot();
                    emptySlot.CurrentItem = backpackItem;
                    emptySlot.CurrentItem.go.transform.SetParent(emptySlot.GetItemContainer);
                    backpackItem.item.MySlot = emptySlot;
                    emptySlot.IsEmpty = false;
                }
            }

            dragingBackpackItems.Clear();
        }

        return true;
    }
    private bool ChangeItemSlot(ItemSlot slotDestination, ItemSlot slotOrigin)
    {
        if (slotDestination.IsEmpty)
        {
            // prirad do volne pozice
            _dragingObject.go.transform.SetParent(slotDestination.GetItemContainer);
            _dragingObject.item.MySlot = slotDestination;
            slotDestination.CurrentItem = _dragingObject;
            slotDestination.IsEmpty = false;

            // original pozice

            if (slotOrigin != slotDestination)
            {
                slotOrigin.CurrentItem = (null, null);
                slotOrigin.IsEmpty = true;
            }

            _originalSlot = null;
            _draggingProceed = false;
        }
        else
        {
            // switching 
            SwitchItems(slotDestination, slotOrigin);

        }

        return true;
    }
    private bool ChangeItemSlot(SpecInventorySlot specSlotDestination2, Slot itemSlotOrigin2)
    {
        if (ItemCanNotBePlacedIntoDestinationSlot(specSlotDestination2) && specSlotDestination2.IsBackpack == false)
        {
            if(itemSlotOrigin2 is SpecInventorySlot originSlotInventory)
            {
                if (_dragingObject.item.Type != originSlotInventory.GetFirstSlotType && _dragingObject.item.Type != originSlotInventory.GetSecondSlotType)
                {
                    ItemSlot emptySlot = inventory.FindEmptySlot();

                    // TODO Edge case
                    if (emptySlot == null)
                        Debug.LogError("Osudova chyba už neni empty slot takže se to musi rozširit inventar GG..");

                    ReturnToOriginSlot(emptySlot);
                    SetDefault();
                    return false;
                }
            }

            // vrat item na puvodni pozici...
            ReturnToOriginSlot(itemSlotOrigin2);
            return false;
        }
        else
        {
            bool isBackpackMoving = false;

            // in progress
            if (_dragingObject.item is Backpack backpack)
            {
                isBackpackMoving = true;
                specSlotDestination2.OpenBackPack(backpack.Capacity);
            }

            if (specSlotDestination2.IsEmpty)
            {
                // prirad do volne pozice
                _dragingObject.go.transform.SetParent(specSlotDestination2.GetItemContainer);
                _dragingObject.item.MySlot = specSlotDestination2;
                specSlotDestination2.CurrentItem = _dragingObject;
                specSlotDestination2.IsEmpty = false;

                // slot empty no longer draging
                _draggingProceed = false;

                // original pozice
                _originalSlot = null;

                if (itemSlotOrigin2 != specSlotDestination2)
                {
                    itemSlotOrigin2.CurrentItem = (null, null);
                    itemSlotOrigin2.IsEmpty = true;
                }

                // pokud se jedna o pohyb batahu
                // premistit itemy z batohu k jinemu charakteru z predesleho mista..
                if (isBackpackMoving)
                {
                    List<SpecInventorySlot> destinySlots = specSlotDestination2.GetSpecialist.GetCharacterBackpackSlots();

                    int i = 0;
                    if (dragingBackpackItems != null && dragingBackpackItems.Count > 0)
                    {
                        foreach (SpecInventorySlot destinySlot in destinySlots)
                        {
                            if (dragingBackpackItems[i] != (null, null))
                            {
                                dragingBackpackItems[i].go.transform.SetParent(destinySlot.GetItemContainer);
                                destinySlot.CurrentItem = dragingBackpackItems[i];
                                destinySlot.IsEmpty = false;
                                destinySlot.CurrentItem.item.MySlot = destinySlot;
                            }
                            i++;
                        }
                    }
                    dragingBackpackItems.Clear();
                }
            }
            else
            {
                var itemFromDestinySlot = specSlotDestination2.CurrentItem;

                _dragingObject.go.transform.SetParent(specSlotDestination2.GetItemContainer);
                specSlotDestination2.CurrentItem = _dragingObject;
                specSlotDestination2.CurrentItem.item.MySlot = itemFromDestinySlot.item.MySlot;
                _dragingObject.item.GetDragAndDropHandler.MakeTransparent(false);

                _dragingObject = itemFromDestinySlot;
                _dragingObject.go.transform.SetParent(dragHolder);
                _dragingObject.item.MySlot = null;
                _dragingObject.item.GetDragAndDropHandler.MakeTransparent(true);

                _draggingProceed = true;
                itemSlotOrigin2.CurrentItem = (null, null);
                itemSlotOrigin2.IsEmpty = true;

                // pokud se jedna o pohyb batohu
                if (isBackpackMoving && itemFromDestinySlot.item is Backpack backpackPrevious)
                {
                    // v dragu mam origin backpakc. Ten si preuložím do tmpListu a pak vymažu dragList backpack
                    List<(Item item, GameObject go)> tmpBackpackItems = new List<(Item item, GameObject go)>();
                    foreach (var item in dragingBackpackItems)
                    {
                        tmpBackpackItems.Add(item);
                    }
                    dragingBackpackItems.Clear();

                    // uložím si destination backpack do DragListu budu potrebovat.
                    List<SpecInventorySlot> destinyBackpackSlots = specSlotDestination2.GetSpecialist.GetCharacterBackpackSlots();
                    foreach (SpecInventorySlot destinySlot in destinyBackpackSlots)
                    {
                        dragingBackpackItems.Add(destinySlot.CurrentItem);

                        if (destinySlot.CurrentItem != (null, null))
                        {
                            destinySlot.CurrentItem.go.transform.SetParent(dragBackpackHolder);
                            destinySlot.IsEmpty = true;
                            destinySlot.CurrentItem = (null, null);
                        }
                    }

                    // vložím do destination backpacku itemy z tmpListu což je origin backPack
                    int i = 0;
                    foreach (SpecInventorySlot destinyBackpackSlot in destinyBackpackSlots)
                    {
                        if (tmpBackpackItems.Count <= 0)
                            break;

                        if (tmpBackpackItems[i] != (null, null))
                        {
                            tmpBackpackItems[i].go.transform.SetParent(destinyBackpackSlot.GetItemContainer);
                            destinyBackpackSlot.CurrentItem = tmpBackpackItems[i];
                            destinyBackpackSlot.CurrentItem.item.MySlot = destinyBackpackSlot;
                            destinyBackpackSlot.IsEmpty = false;
                        }
                        i++;
                    }

                    tmpBackpackItems.Clear();

                }

                    
                // vymenit batohy.. i itemy ve vnitř.
            }
        }

        return true;
    }
    private void SwitchItems(ItemSlot itemSlotDestination, SpecInventorySlot specSlotOrigin2)
    {

        var tmp = itemSlotDestination.CurrentItem;

        _dragingObject.go.transform.SetParent(itemSlotDestination.GetItemContainer);
        _dragingObject.item.MySlot = itemSlotDestination;
        itemSlotDestination.CurrentItem = _dragingObject;
        _dragingObject.item.GetDragAndDropHandler.MakeTransparent(false);

        _dragingObject = tmp;
        _dragingObject.go.transform.SetParent(dragHolder);
        _dragingObject.item.MySlot = null;
        _dragingObject.go.GetComponent<DragAndDropHandler>().MakeTransparent(true);

        IsDraggingProceed = true;
        specSlotOrigin2.CurrentItem = (null, null);
        specSlotOrigin2.IsEmpty = true;

    }
    private void SwitchItems(ItemSlot slotDestination, ItemSlot slotOrigin)
    {
        var itemFromDestinySlot = slotDestination.CurrentItem;

        _dragingObject.go.transform.SetParent(slotDestination.GetItemContainer);
        _dragingObject.item.MySlot = slotDestination;
        _dragingObject.item.GetDragAndDropHandler.MakeTransparent(false);
        slotDestination.CurrentItem = _dragingObject;

        _dragingObject = itemFromDestinySlot;
        _dragingObject.go.transform.SetParent(dragHolder);
        _dragingObject.item.MySlot = null;
        _dragingObject.item.GetDragAndDropHandler.MakeTransparent(true);

        _draggingProceed = true;

        slotOrigin.CurrentItem = (null, null);
        slotOrigin.IsEmpty = true;
    }
    private void ReturnToOriginSlot(Slot specSlotOrigin2)
    {
        if (specSlotOrigin2 == null) return;

        _dragingObject.go.transform.SetParent(specSlotOrigin2.GetItemContainer);
        _dragingObject.item.GetDragAndDropHandler.MakeTransparent(false);
        _dragingObject.item.MySlot = specSlotOrigin2;
        specSlotOrigin2.IsEmpty = false;
        specSlotOrigin2.CurrentItem = _dragingObject;

        if (specSlotOrigin2 is SpecInventorySlot inventorySlot && _dragingObject.item is Backpack backpack)
        {
            List<SpecInventorySlot> charBackpackSlots = inventorySlot.GetSpecialist.GetCharacterBackpackSlots();

            int i = 0;
            foreach (SpecInventorySlot slot in charBackpackSlots)
            {
                slot.CurrentItem = dragingBackpackItems[i];

                if (dragingBackpackItems[i] != (null, null))
                {
                    dragingBackpackItems[i].go.transform.SetParent(slot.GetItemContainer);
                    slot.IsEmpty = false;
                }
              
                i++;
            }
            dragingBackpackItems.Clear();

            inventorySlot.OpenBackPack(backpack.Capacity);
        }
    }

    private void ReturnToOriginSlot()
    {
        _dragingObject.go.transform.SetParent(_originalSlot.GetItemContainer);
        _dragingObject.item.MySlot = _originalSlot;
        _originalSlot.CurrentItem = _dragingObject;
        _dragingObject.item.GetDragAndDropHandler.MakeTransparent(false);
        _originalSlot.IsEmpty = false;

        if (_originalSlot is SpecInventorySlot inventorySlot && _dragingObject.item is Backpack backpack)
        {
            List<SpecInventorySlot> charBackpackSlots = inventorySlot.GetSpecialist.GetCharacterBackpackSlots();

            int i = 0;
            foreach (SpecInventorySlot slot in charBackpackSlots)
            {
                if (dragingBackpackItems.Count < 1)
                    break;

                if (i >= dragingBackpackItems.Count)
                    continue;

                slot.CurrentItem = dragingBackpackItems[i];

                if (dragingBackpackItems[i] != (null, null))
                {
                    dragingBackpackItems[i].go.transform.SetParent(slot.GetItemContainer);
                    slot.IsEmpty = false;
                }

                i++;
            }
            dragingBackpackItems.Clear();

            inventorySlot.OpenBackPack(backpack.Capacity);
        }

    }
    private bool ItemCanNotBePlacedIntoDestinationSlot(SpecInventorySlot specSlotDestination2)
    {
        bool result = (_dragingObject.item.Type != specSlotDestination2.GetFirstSlotType && _dragingObject.item.Type != specSlotDestination2.GetSecondSlotType);
        return result;
    }
}
