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

    private Slot mySlot;  

    private ItemType type;

    private int capacity = 0;

    [SerializeField] private Image image;

    public string Name { get { return this.name; } set { name = value; } }

    public Slot MySlot { get { return this.mySlot; } set { mySlot = value; } }

    public Sprite Sprite { get { return this.image.sprite; } set { image.sprite = value; } }

    public ItemType Type { get { return this.type; } }

    public int Capacity { get { return this.capacity; } set { this.capacity = value; } }
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

    /// od itemu bude base class.. A pak dalsi tridy jako Bagg weapon ad...

}
