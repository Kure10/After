using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    public List<SpecInventorySlot> slots = new List<SpecInventorySlot>();

    public void AddItem(SpecInventorySlot slot)
    {
        slots.Add(slot);
    }

    public void RemoveItem(SpecInventorySlot slot)
    {
        slots.Remove(slot);
    }


}
