using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemBlueprint;

public class Weapon : Item
{
    private int useCount;
    private bool isRepairable;
    private RepairCost repairCost;

    private int repairBlock;
    private int rangeMin;
    private int rangeMax;

    private bool isIndestructible = false;

    public int UseCount { get { return this.useCount; } set { this.useCount = value; } }
    public bool IsRepairable { get { return this.isRepairable; } set { this.isRepairable = value; } }
    public RepairCost RepairCost { get { return this.repairCost; } set { this.repairCost = value; } }

    public int RepairBlock { get { return this.repairBlock; } set { this.repairBlock = value; } }
    public int RangeMin { get { return this.rangeMin; } set { this.rangeMin = value; } }
    public int RangeMax { get { return this.rangeMax; } set { this.rangeMax = value; } }

    public bool IsIndestructible { get { return this.isIndestructible; } set { this.isIndestructible = value; } }

    public void SetupItem(int _useCount, bool _isRepairable, string _name, ItemType _type, Sprite _sprite)
    {
        base.SetupItem(_name, _type, _sprite);

        useCount = _useCount;
        isRepairable = _isRepairable;
    }

    // testing
    public Weapon ()
    {

    }
    // testing
    public Weapon(int maxRange , int minRange , int useCount, int power)
    {
        this.rangeMax = maxRange;
        this.rangeMin = minRange;
        this.useCount = useCount;

        BonusModificators bm = new BonusModificators();
        bm.AtributeModificator = AtributeModificator.MiL;
        bm.AtributeChangeVal = power;
        bm.MathKind = MathKind.plus;

        bm.TestModificator = TestModificator.None;
        bm.TypeModificator = TypeModificator.DiceCountMod;

        Modificators = new BonusModificators[1];
        Modificators[0] = bm;
    }

}
