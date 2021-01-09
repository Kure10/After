﻿using System.Collections;
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

    private ItemType type;

    [SerializeField] private Image image;

    private Slot mySlot;

    public string Name { get { return this.name; } set { name = value; } }

    public Slot MySlot { get { return this.mySlot; } set { mySlot = value; } }

    public Sprite Sprite { get { return this.image.sprite; } set { image.sprite = value; } }

    public ItemType Type { get { return this.type; } }

    public void SetupItem(string _name, ItemType _type, Sprite _sprite)
    {
        name = _name;
        image.sprite = _sprite;
        type = _type;
    }


    private void Awake()
    {
        image = this.transform.GetChild(0).gameObject.GetComponent<Image>();
    }

    //public Item(string _name, ItemType _type, Sprite _sprite)
    //{
    //    name = _name;
    //    image.sprite = _sprite;
    //    type = _type;
    //}
}