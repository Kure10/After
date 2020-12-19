using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SpecInventorySlot : Slot  
{
    [SerializeField] int _index;

    [SerializeField] ItemBlueprint.ItemType type;
    // Todo zatim se nepouziva..
    [SerializeField] Image backgroundImage;

    [SerializeField] GameObject selectedBackground;

    public event Action<Item, SpecInventorySlot> OnItemChangeCallBack = delegate { };

    public ItemBlueprint.ItemType GetSlotType { get { return this.type; } }

    public int GetIndex { get { return this._index; } }

    // budu potrebovat
    public override (Item item, GameObject go) CurrentItem
    {
        get
        {
            return _currentItem;
        }
        set
        {
            _currentItem = value;
            OnItemChangeCallBack?.Invoke(_currentItem.item, this);
        }
    }

    public void ShowDragPosibility()
    {
        selectedBackground.gameObject.SetActive(true);
    }

    public void HideDragPosibility()
    {
        selectedBackground.gameObject.SetActive(false);
    }


}
