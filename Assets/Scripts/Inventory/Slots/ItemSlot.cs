using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : Slot  
{
    [SerializeField] Image image;

    public void AddItem(GameObject gameObject, Item item)
    {
        gameObject.transform.SetParent(container);
 
        isEmpty = false;
        this.CurrentItem = (item, gameObject);

        
    }

    public void SetEmpty()
    {
        image.gameObject.SetActive(false);
    }

    //override public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (DragAndDropManager.IsDraging)
    //    {
    //        DragAndDropManager.Instantion.SetDropPosition(this);
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
    //}
}
