using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Slot : MonoBehaviour, IPointerHandler 
{
    [SerializeField] protected Transform container;

    protected bool isEmpty = true;

    protected (Item item, GameObject go) currentItem;

    #region Property
    public Transform GetItemContainer { get { return container; } }

    public bool IsEmpty { get { return isEmpty; } set { isEmpty = value; } }

    public (Item item, GameObject go) CurrentItem { get { return currentItem; } set { currentItem = value; } }

    #endregion
    abstract public void OnPointerEnter(PointerEventData eventData);


    abstract public void OnPointerExit(PointerEventData eventData);


    abstract public void OnPointerClick(PointerEventData eventData);

}
