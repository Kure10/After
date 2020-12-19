using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropHandler : MonoBehaviour, IPointerHandler, IDragable , IDropHandler
{
    private CanvasGroup canvasGroup;
 
    private ( Item item, GameObject go) itemInSlot;

    // Zatím Todo. Mozna bude mit drag and drop vlastni canvas.. uvidím.
    //private Canvas canvas;
    //private RectTransform rect;


    private void Awake()
    {
        itemInSlot.go = this.gameObject;
        itemInSlot.item = this.gameObject.GetComponent<Item>();
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();

        //rect = GetComponent<RectTransform>();
        //canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
        DragAndDropManager.Instantion.InitDraging(itemInSlot);
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (DragAndDropManager.IsDraging)
        {
            Vector3 posMouse = Input.mousePosition;
            this.transform.position = posMouse;

            //rect.anchoredPosition += eventData.delta / canvas.scaleFactor;

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        DragAndDropManager.IsDraging = false;

        DragAndDropManager.Instantion.wasSuccessfullyDroped();

        DragAndDropManager.Instantion.SetDefault();

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
        Debug.Log("OnDrop");
        DragAndDropManager.Instantion.HandleDrop(itemInSlot.item.MySlot);
    }
}
