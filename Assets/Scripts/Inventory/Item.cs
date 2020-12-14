using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using static ItemBlueprint;

public class Item : MonoBehaviour
{
    private Slot mySlot;  // odstranit Ownera zapremyslet..

    private ItemType type;

    public ItemBlueprint blueprint;

    [SerializeField] private Image image;

    public ItemBlueprint Blueprint { get { return this.blueprint; } set { blueprint = value; } }

    public Slot MySlot { get { return this.mySlot; } set { mySlot = value; } }

    public Sprite Sprite { get { return this.image.sprite; } set { image.sprite = value; } }

    public new ItemType GetType { get { return this.type; } }

    public ItemType SetType { set { this.type = value; } }


    // todo Vic...
    public Item(ItemType _type)
    {
        type = _type;
    }

}
