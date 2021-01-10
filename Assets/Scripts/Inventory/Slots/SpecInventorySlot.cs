﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SpecInventorySlot : Slot  
{
    [SerializeField] int _index;

    [SerializeField] ItemBlueprint.ItemType firstType;
    [SerializeField] ItemBlueprint.ItemType secondType;
    // Todo zatim se nepouziva..
    [SerializeField] Image backgroundImage;

    [SerializeField] GameObject selectedBackground;

    public event Action<Item, SpecInventorySlot> OnItemChangeCallBack = delegate { };

    public ItemBlueprint.ItemType GetFirstSlotType { get { return this.firstType; } }

    public ItemBlueprint.ItemType GetSecondSlotType { get { return this.secondType; } }

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

    public void SetSlot(GameObject gameObject, Item item)
    {
        _index = 0;
        isEmpty = false;
        this.CurrentItem = (item, gameObject);
        gameObject.transform.SetParent(container);
    }

}
