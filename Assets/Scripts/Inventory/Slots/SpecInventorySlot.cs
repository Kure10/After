using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class SpecInventorySlot : Slot
{
    [Header("Specific")]

    [SerializeField] bool isBackPack;

    [SerializeField] ItemBlueprint.ItemType firstType;
    [SerializeField] ItemBlueprint.ItemType secondType;
    [Space]
    // Todo zatim se nepouziva..
    [SerializeField] Image backgroundImage;

    [SerializeField] GameObject selectedBackground;


    public event Action<Item, SpecInventorySlot> OnItemChangeCallBack = delegate { };

    public ItemBlueprint.ItemType GetFirstSlotType { get { return this.firstType; } }

    public ItemBlueprint.ItemType GetSecondSlotType { get { return this.secondType; } }

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
        isEmpty = false;
        this.CurrentItem = (item, gameObject);
        gameObject.transform.SetParent(container);
    }

    public override void AddAction(UnityAction<int> action, UnityAction action2)
    {
        if (!isEmpty)
        {
            if (_currentItem.item is Backpack back)
            {
                actionButton.onClick.AddListener(delegate { action(back.Capacity); });
                actionButton.onClick.AddListener(delegate { action2(); });
            }
        }
    }

    public void OpenBackPack()
    {

    }

}
