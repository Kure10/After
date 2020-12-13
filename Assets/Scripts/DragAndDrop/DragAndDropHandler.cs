using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropHandler : MonoBehaviour, IPointerHandler, IDragable
{
    private CanvasGroup canvasGroup;
 
    private ( Item item, GameObject go) dragingObject;

    // Zatím Todo. Mozna bude mit drag and drop vlastni canvas.. uvidím.
    //private Canvas canvas;
    //private RectTransform rect;


    private void Awake()
    {
        dragingObject.go = this.gameObject;
        dragingObject.item = this.gameObject.GetComponent<Item>();
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();

        //rect = GetComponent<RectTransform>();
        //canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        DragAndDropManager.Instantion.InitDraging(dragingObject);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
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

        DragAndDropManager.IsDraging = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;


        bool isPosibleDrop = DragAndDropManager.Instantion.IsDropItemPosible();
        bool result;

        if (!isPosibleDrop)
        {
            result = DragAndDropManager.Instantion.ReturnToOriginalPosition();
        }
        else
        {
            result = DragAndDropManager.Instantion.SwitchPosition();
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
}
