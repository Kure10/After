using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SpecInventorySlot : Slot  
{
    [SerializeField] ItemBlueprint.ItemType type;
    // Todo zatim se nepouziva..
    [SerializeField] Image backgroundImage;

    public event Action<Item> OnEventEnd = delegate { };

    public override (Item item, GameObject go) CurrentItem
    {
        get
        {
            return _currentItem;
        }
        set
        {
            _currentItem = value;
            OnEventEnd?.Invoke(_currentItem.item);
        }
    }

    public ItemBlueprint.ItemType GetSlotType { get { return this.type; } }

}
