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

    //override public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (DragAndDropManager.IsDraging)
    //    {
    //        var dragingObject = DragAndDropManager.Instantion.GetDragingObject;

    //        if (type == dragingObject.item.Blueprint.Type)
    //        {
    //            DragAndDropManager.Instantion.SetDropPosition(this);
    //        }
    //        else
    //        {
    //            DragAndDropManager.Instantion.SetDropPosition(null);
    //        }
    //    }
    //    else
    //    {
    //        DragAndDropManager.Instantion.SetOriginSlot(this);
    //    }
    //}

    //override public void OnPointerExit(PointerEventData eventData)
    //{
    //    if (DragAndDropManager.IsDraging)
    //    {
    //        DragAndDropManager.Instantion.SetDropPosition(null);
    //    }
    //    //else
    //    //{
    //    //    DragAndDropManager.Instantion.SetOriginSlot(null);
    //    //}
    //}

}
