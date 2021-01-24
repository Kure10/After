using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

[Serializable]
public abstract class Slot : MonoBehaviour, IPointerHandler, IDropHandler
{
    [Header("Main")]
    [SerializeField] protected Transform container;

    [SerializeField] protected Button actionButton;

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

    private void Awake()
    {
        actionButton = this.gameObject.GetComponent<Button>();
    }

    // Todo tohle bude asi abstraktni..
    public virtual void AddAction(UnityAction<int> action, UnityAction action2)
    {
        actionButton.onClick.AddListener(delegate { action(-1);});
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // možna budu potrebovat zatím nechej byt.. Zaleži jestli se bug obeví.
        // Draguji kurzorem do stredu nejakeho itemu a pak zkusím dragovat znova s jinym itemem..
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragAndDropManager.Instantion.HandleDrop(this);
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
