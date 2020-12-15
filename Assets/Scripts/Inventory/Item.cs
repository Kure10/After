using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using static ItemBlueprint;
using System;

[Serializable]
public class Item : MonoBehaviour
{
    private string name;

    private Slot mySlot;  // odstranit Ownera zapremyslet..

    private ItemType type;

    [SerializeField] private Image image;

    public Slot MySlot { get { return this.mySlot; } set { mySlot = value; } }

    public Sprite Sprite { get { return this.image.sprite; } set { image.sprite = value; } }

    public ItemType Type { get { return this.type; } }

    public void SetupItem(string _name, ItemType _type, Sprite _sprite)
    {
        name = _name;
        image.sprite = _sprite;
        type = _type;
    }

}
