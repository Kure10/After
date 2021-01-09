using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemBlueprint;

public class Backpack : Item
{
    private int capacity;

    public int Capacity { get { return this.capacity; } set { this.capacity = value; } }

    public void SetupItem(int _capacity, string _name, ItemType _type, Sprite _sprite)
    {
        base.SetupItem(_name, _type, _sprite);

        capacity = _capacity;
    }

    //public Backpack(int _capacity, string _name, ItemType _type, Sprite _sprite) : base(_name, _type, _sprite)
    //{
    //    capacity = _capacity;
    //}
}
