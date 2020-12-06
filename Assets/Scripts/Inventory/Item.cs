using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Item
{
    public long itemID;
    public new string name;
    public ItemType type;

    public long Id { get { return itemID; } }
    public string Name { get { return name; } }

    public ItemType Type { get { return type; } }

    public Item(long _id, string _name, ItemType _type)
    {
        itemID = _id;
        name = _name;
        type = _type;
    }

    public enum ItemType
    {
        ArmorSpec,
        BagSpec,
        ItemSpec,
        ResBasic,
        WeapSpec
    }

}
