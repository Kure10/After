using UnityEngine;
using System;

[Serializable]
public class ItemBlueprint
{
    // Todo make private
    public long itemID;
    public new string name;
    private Sprite sprite;
    private ItemType type;
    private ItemResourceType resourceType = ItemResourceType.None;
    public int capacity;
    public int absorbation;
    public bool isRepairable;
    public bool isIndestructible = false;
    public int repairBlock;
    public int useCount;
    public int rangeMin = 0;
    public int rangeMax = 0;
    public int sizeStock = 1;

    public RepairCost repairCost;
    public BonusModificators[] modificators;

    public Sprite Sprite { get { return sprite; } set { sprite = value; } }

    public ItemType Type { get { return type; } }

    public ItemResourceType ResourceType { get { return resourceType; } set { resourceType = value; } }

    public ItemBlueprint()
    {

    }

    public ItemBlueprint(long _id, string _name, ItemType _type)
    {
        itemID = _id;
        name = _name;
        type = _type;
    }

    public enum ItemType
    {
        None,
        ArmorSpec,
        BagSpec,
        ItemSpec,
        ResBasic,
        ResSpecial,
        WeapSpec
    }

    public enum ItemResourceType
    {
        None,
        Civil,
        Military,
        Fuel,
        Food,
        TechMaterial
    }

    public struct BonusModificators
    {
        public AtributeModificator AtributeModificator;
        public int AtributeChangeVal;
        public TestModificator TestModificator;
        public int TestChangeVal;
        public MathKind MathKind;
        public TypeModificator TypeModificator;
    }

    public struct RepairCost
    {
        public int TM;
        public int MM;
        public int CM;
    }

    #region Struct Enum
    public enum TestModificator
    {
        None,
        Battle,
        Comunication,
        Hunting,
        Repair,
        LockPicking,
        Leverage,
        Sneaking,
        Scouting,
        ScoutingBuild,
        Scavenging,
        Research,
        DiggBuild,
        Selection
    }

    public enum MathKind
    {
        None,
        plus,
        minus,
        times
    }

    public enum TypeModificator
    {
        None,
        DiceCountMod,
        DiffChange
    }

    public enum AtributeModificator
    {
        None,
        MiL,
        SoL,
        Scl,
        Tel,
        LvL,
        Special
    }

    #endregion

}
