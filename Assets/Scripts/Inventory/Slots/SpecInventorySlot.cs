using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpecInventorySlot : Slot  
{
    [SerializeField] ItemBlueprint.ItemType type;
    // Todo zatim se nepouziva..
    [SerializeField] Image backgroundImage;

    public ItemBlueprint.ItemType GetSlotType { get { return this.type; } }

}
