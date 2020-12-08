using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecInventorySlot : MonoBehaviour
{
    [SerializeField] Image backgroundImage;

    [SerializeField] Transform container;

    [SerializeField] Button button;

    private ItemSlot itemSlot = null;


    public void SetItemSlot (ItemSlot slot)
    {
        if (slot == null)
            return;

        itemSlot = slot;
        slot.transform.SetParent(container);

        SetInventoryOccupied();
    }

    public ItemSlot UnSetItemSlot()
    {
        if (itemSlot == null)
            return null;

        itemSlot = null;
        

        SetInventoryEmpty();

        return itemSlot;
    }

    private void SetInventoryOccupied()
    {
        backgroundImage.gameObject.SetActive(false);
    }

    private void SetInventoryEmpty()
    {
        backgroundImage.gameObject.SetActive(true);
    }

}
