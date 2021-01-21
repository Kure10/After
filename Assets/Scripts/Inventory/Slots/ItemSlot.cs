using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class ItemSlot : Slot  
{
    [Header("Specific")]
    [SerializeField] Image image;

    private int _index = 0; // index je asi k hovnu..

    public int Index { get { return _index; } set { _index = value; } }

    public void SetSlot(int index, GameObject gameObject, Item item)
    {
        _index = index;
        isEmpty = true;
        image.gameObject.SetActive(false);
        this.CurrentItem = (item, gameObject);

        if (gameObject != null && item != null)
        {
            gameObject.transform.SetParent(container);
            isEmpty = false;
        }
    }

}
