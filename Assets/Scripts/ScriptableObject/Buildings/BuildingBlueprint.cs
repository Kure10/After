using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Building", fileName = "_NewBuilding")]
public class BuildingBlueprint : ScriptableObject
{
    [Header("Main Settings")]
    private long id;
    [SerializeField] string buildingName = "Default";
    [SerializeField]
    Tag tag = Tag.Ubikace;
    [SerializeField]
    TypeOfBuilding type = TypeOfBuilding.Basis;
    [Tooltip("v sekundach")]
    [SerializeField] float timeToBuild = 20f;
    
    [Header("Building requirements")]
    [SerializeField] private float electricConsumption = 0;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject constructionPrefab;
    [SerializeField] private Color backgroundColor;

    //prozatim quick & dirty, at se da stavet
    [SerializeField] public int column;
    [SerializeField] public int row;

    [Header("Resources cost")]
    [SerializeField] int civilniMaterial = 0;
    [SerializeField] int technickyMaterial = 0;
    [SerializeField] int vojenskyMaterial = 0;
    [Space]
    //[SerializeField] private int rawMaterial = 0; // Tohle bude bool -> Když ano tak by se meli zobrazit jeden raw material a tlacitko plus na pridani dalsiho
    [Header("Images")]
    [SerializeField] Sprite ilustrationImage;
    [Header("Text Information")]
    [TextArea(4, 10)]
    [SerializeField] string textForInfo = "Lazy GameDesign";

    public List<RawMaterials> listRawMaterials = new List<RawMaterials>();

    public long Id { get { return this.id; } set { this.id = value; } }
    public string Name { get { return buildingName; } set { buildingName = value; } }
    public int Civil { get { return civilniMaterial; } set { civilniMaterial = value; } }
    public int Tech { get { return technickyMaterial; } set { technickyMaterial = value; } }
    public int Military { get { return vojenskyMaterial; } set { vojenskyMaterial = value; } }
    public Sprite Sprite { get { return ilustrationImage; } set { ilustrationImage = value; } }
    public int Size { get { return row * column; } }
    public int RowSize { get { return row; } }
    public int ColumnSize { get { return column; } }
    public string Info { get { return textForInfo; } set { textForInfo = value; } }
    public float TimeToBuild { get { return timeToBuild; } set { timeToBuild = value; } }
    public float ElectricConsumption { get { return electricConsumption; } set { electricConsumption = value; } }
    public GameObject Prefab { get { return prefab; } set { prefab = value; } }
    public GameObject ConstructionPrefab { get { return constructionPrefab; } set { constructionPrefab = value; } }

    public Tag Tag { get { return this.tag; } set { this.tag = value; } }

    public TypeOfBuilding Type { get { return this.type; } set { this.type = value; } }

    public Color BackgroundColor
    {
        get => backgroundColor;
        set => backgroundColor = value;
    }

    public List<RawMaterials> GetĹistRawMaterials()
    {
        return listRawMaterials;
    }

    public int GetCountRawMaterials()
    {
        return listRawMaterials.Count;
    }

    public void SynchronizedList (List<RawMaterials> myList)
    {
        listRawMaterials = myList;
    }

    public Tag GetTag()
    {
        return tag;
    }

    public void SetColor(int red, int green, int blue)
    {
        backgroundColor = new Color(red/255f, green/255f, blue/255f, 1f );
    }

    public Tag ConvertTagStringData(string data)
    {
        switch (data)
        {
            case "Quarters":
                return Tag.Ubikace;
            case "Store":
                return Tag.Sklad;
            case "EngineRoom":
                return Tag.Strojovna;
            case "WorkShop":
                return Tag.Dilna;
            case "Laboratory":
                return Tag.Laborator;
            case "Loophole":
                return Tag.Strilna;
            case "Cell":
                return Tag.Cela;
            case "Garage":
                return Tag.Garaz;
            case "Chapel":
                return Tag.Kaple;
            default:
                return Tag.VolnePole;
        }
    }

    public TypeOfBuilding ConvertTypeStringData(string data)
    {
        switch (data)
        {
            case "RomeCore":
                return TypeOfBuilding.Basis;
            case "Poust":
                return TypeOfBuilding.Extension;
            default:
                return TypeOfBuilding.Upgrade;
        }
    }


}

public enum TypeOfBuilding {Basis , Extension, Upgrade};

public enum Tag { Ubikace, Strojovna, Dilna, Sklad, Laborator, Strilna, Cela, Garaz, Kaple, VolnePole};

public enum RawMaterials { bojler, varic, lednicka, usmernovac, sadaNaradi, sverak, kovadlina , rozsireneNaradi, elektrickeRucniNastroje, strojniZarizeni, automatizacniJednotka, Computer , zakladniNastroje,None };
