using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : Slot , IDropHandler 
{
    [SerializeField] Image image;

    public void AddItem(GameObject gameObject, Item item)
    {
        gameObject.transform.SetParent(container);
        item.Owner = this;
        isEmpty = false;
        this.CurrentItem = (item, gameObject);

        Debug.Log(this);

    }

    public void SetEmpty()
    {
        image.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    override public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragAndDropManager.IsDraging)
        {
            DragAndDropManager.Instantion.SetDropPosition(this);
        }
    }

    override public void OnPointerExit(PointerEventData eventData)
    {
        if (DragAndDropManager.IsDraging)
        {
            DragAndDropManager.Instantion.SetDropPosition(null);
        }
    }

    override public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
