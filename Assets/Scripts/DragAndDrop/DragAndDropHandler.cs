using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropHandler : MonoBehaviour, IPointerHandler, IDragable , IDropHandler
{
    private CanvasGroup canvasGroup;
 
    private ( Item item, GameObject go) itemInSlot;

    [SerializeField] public bool _disableDrag = false;

    public void InitDragHandler()
    {
        itemInSlot.go = this.gameObject;
        itemInSlot.item = this.gameObject.GetComponent<Item>();
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_disableDrag)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.7f;
            DragAndDropManager.Instantion.InitDraging(itemInSlot);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_disableDrag)
        {
            if (DragAndDropManager.IsDraging)
            {
                Vector3 posMouse = Input.mousePosition;
                this.transform.position = posMouse;

                //rect.anchoredPosition += eventData.delta / canvas.scaleFactor;

            }
        }   
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_disableDrag)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            DragAndDropManager.IsDraging = false;

            DragAndDropManager.Instantion.wasSuccessfullyDroped();

            DragAndDropManager.Instantion.SetDefault();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_disableDrag)
        {
            DragAndDropManager.Instantion.HandleDrop(itemInSlot.item.MySlot);
        }
    }
}
