using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpecInventorySlot : MonoBehaviour , IDropHandler
{
    [SerializeField] Image backgroundImage;

    [SerializeField] Transform container;

    [SerializeField] Button button;


    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        GameObject dragGo = eventData.pointerDrag;
        Item item = dragGo.GetComponent<Item>();
        if (item != null)
        {
            dragGo.transform.SetParent(container);
            backgroundImage.gameObject.SetActive(false);
        }


    }
}
