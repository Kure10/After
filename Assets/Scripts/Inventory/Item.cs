using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Item : MonoBehaviour
{
    private Slot owner;  // odstranit Ownera zapremyslet..

    public ItemBlueprint blueprint;

    [SerializeField] private Image image;

    public ItemBlueprint Blueprint { get { return this.blueprint; } set { blueprint = value; } }

    public Slot Owner { get { return this.owner; } set { owner = value; } }

    public Sprite Sprite { get { return this.image.sprite; } set { image.sprite = value; } }

}
