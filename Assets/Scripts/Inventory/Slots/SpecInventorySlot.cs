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

    [SerializeField] private uWindowSpecialist specialist;

   // public int aaaa = 0;
    [SerializeField] BodyPart bodyPart;

    [SerializeField] private bool isBackPack;

    [SerializeField] private List<ItemBlueprint.ItemType> slotTypes;
    [Space]
    // Todo zatim se nepouziva..
    [SerializeField] Image backgroundImage;

    [SerializeField] GameObject selectedBackground;

    public uWindowSpecialist GetSpecialist { get { return this.specialist; } }

    public event Action<Item, GameObject, SpecInventorySlot> OnItemChangeCallBack = delegate { };

    // tady pridat akci na open back pack
    public event Action<int> OnOpenBackPack = delegate { };

    public event Action OnCloseBackPack = delegate { };

    public event UnityAction OnGridSizeChange = delegate { };

    public List<ItemBlueprint.ItemType> GetSlotTypes { get { return this.slotTypes;} }    

    public BodyPart GetBodyPart { get { return this.bodyPart; } }

    public bool IsBackpack { get { return this.isBackPack; } }
    public override (Item item, GameObject go) CurrentItem
    {
        get
        {
            return _currentItem;
        }
        set
        {
            _currentItem = value;
            OnItemChangeCallBack?.Invoke(_currentItem.item, _currentItem.go, this);
        }
    }

    public (Item item, GameObject go) SerCurrentItemWithoutNotify
    {
        get
        {
            return _currentItem;
        }
        set
        {
            _currentItem = value;
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

    public void CloseBackpack()
    {
        OnCloseBackPack?.Invoke();
        OnGridSizeChange?.Invoke();
    }

    public void OpenBackPack(int capacity)
    {
        OnOpenBackPack?.Invoke(capacity);
        OnGridSizeChange?.Invoke();
    }

    public bool HasSlotThatType(ItemBlueprint.ItemType type)
    {
        bool result = false;
        foreach (ItemBlueprint.ItemType slotType in GetSlotTypes)
        {
            if (type == slotType)
            {
                result = true;
            }
        }

        return result;
    }

    public enum BodyPart
    {
        Back,
        Chest,
        LeftHand,
        RightHand
    }
}


