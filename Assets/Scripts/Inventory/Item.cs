using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour
{
    private Slot mySlot;  // odstranit Ownera zapremyslet..

    public ItemBlueprint blueprint;

    [SerializeField] private Image image;

    public ItemBlueprint Blueprint { get { return this.blueprint; } set { blueprint = value; } }

    public Slot MySlot { get { return this.mySlot; } set { mySlot = value; } }

    public Sprite Sprite { get { return this.image.sprite; } set { image.sprite = value; } }

}
