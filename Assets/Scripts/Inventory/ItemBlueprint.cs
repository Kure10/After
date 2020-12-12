using UnityEngine;
using System;

[Serializable]
public class ItemBlueprint
{
    // Todo make private
    public long itemID;
    public new string name;
    public Sprite sprite;
    public ItemType type;

    public long Id { get { return itemID; } }
    public string Name { get { return name; } }

    public Sprite Sprite { get { return sprite; } set { sprite = value; } }

    public ItemType Type { get { return type; } }

    public ItemBlueprint(long _id, string _name, ItemType _type)
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
