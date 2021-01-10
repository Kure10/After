using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemBlueprint;

public class ActiveItem : Item
{
    private int useCount;
    private bool isRepairable;
    private RepairCost repairCost;

    private bool isIndestructible = false;

    public int UseCount { get { return this.useCount; } set { this.useCount = value; } }
    public bool IsRepairable { get { return this.isRepairable; } set { this.isRepairable = value; } }
    public RepairCost RepairCost { get { return this.repairCost; } set { this.repairCost = value; } }

    public bool IsIndestructible { get { return this.isIndestructible; } set { this.isIndestructible = value; } }

    public void SetupItem(int _useCount, bool _isRepairable, string _name, ItemType _type, Sprite _sprite)
    {
        base.SetupItem(_name, _type, _sprite);

        useCount = _useCount;
        isRepairable = _isRepairable;
    }

}
