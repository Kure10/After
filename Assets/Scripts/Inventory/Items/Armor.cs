using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemBlueprint;

public class Armor : Item
{
    private int absorbation;
    private bool isRepairable;

    private RepairCost repairCost;
    private int repairBlock;

    public int Absorbation { get { return this.absorbation; } set { this.absorbation = value; } }
    public int RepairBlock { get { return this.repairBlock; } set { this.repairBlock = value; } }
    public bool IsRepairable { get { return this.isRepairable; } set { this.isRepairable = value; } }
    public RepairCost RepairCost { get { return this.repairCost; } set { this.repairCost = value; } }

    //public Armor(int _absorbation, bool _isRepairable, string _name, ItemType _type, Sprite _sprite) : base(_name, _type, _sprite)
    //{
    //    absorbation = _absorbation;
    //    isRepairable = _isRepairable;
    //}

    public void SetupItem(int _absorbation, bool _isRepairable, string _name, ItemType _type, Sprite _sprite)
    {
        base.SetupItem(_name,_type,_sprite);

        absorbation = _absorbation;
        isRepairable = _isRepairable;
    }

}
