using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

[Serializable]
public abstract class Slot : MonoBehaviour, IPointerHandler , IDropHandler
{
    [SerializeField] protected Transform container;

    protected bool isEmpty = true;

    protected (Item item, GameObject go) _currentItem;


    #region Property
    public Transform GetItemContainer { get { return container; } }

    public bool IsEmpty { get { return isEmpty; } set { isEmpty = value; } }

    public virtual (Item item, GameObject go) CurrentItem 
    {
        get
        { 
            return _currentItem;
        } 
        set 
        {
            _currentItem = value;
        }
    }

    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        // možna budu potrebovat zatím nechej byt.. Zaleži jestli se bug obeví.
        // Draguji kurzorem do stredu nejakeho itemu a pak zkusím dragovat znova s jinym itemem..
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragAndDropManager.Instantion.HandleDrop(this);

        Debug.Log("OnDrop");
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
       // throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       // throw new System.NotImplementedException();
    }
}
