using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropHandler : MonoBehaviour, IPointerHandler, IDragable
{
    private CanvasGroup canvasGroup;
    private Item slot;
    private void Awake()
    {
        slot = this.gameObject.GetComponent<Item>();
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        DragAndDropManager.IsDraging = true;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragAndDropManager.IsDraging)
        {
            Vector3 posMouse = Input.mousePosition;
            this.transform.position = posMouse;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragAndDropManager.IsDraging = false;

        canvasGroup.blocksRaycasts = true;

        // skonci Drag
        // cheknout kde jsem zkoncil
        // jestli je prazdny tak priradim slot.  // jestli je plny vymenim  // jestli do sraček priradím. vratím na puvodní místo

        // nasledně stop Draging je false 
        // vycistím data. na dalsí Drag

    }

    public void OnPointerClick(PointerEventData eventData)
    {
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!DragAndDropManager.IsDraging)
        {
           DragAndDropManager.InitDraging(this.gameObject, slot);
        }

        if (DragAndDropManager.IsDraging)
        {
            if (slot.gameObject == eventData.pointerEnter)
                return;

            var pos = eventData.position;

            var ss = eventData.selectedObject;

            var sda = eventData.pointerEnter;

            var sddsaa = eventData.pointerPressRaycast;

            RaycastHit2D hit = Physics2D.Raycast(eventData.position,transform.TransformDirection(Vector2.up));



           // DragAndDropManager.SetDestination(slot.myCurrentPosition);
            

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
