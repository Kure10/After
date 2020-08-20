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
    Sector sector = Sector.Ubikace;
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
    public string Info { get { return textForInfo; } set { textForInfo = value; } }
    public float TimeToBuild { get { return timeToBuild; } set { timeToBuild = value; } }
    public float ElectricConsumption { get { return electricConsumption; } set { electricConsumption = value; } }
    public GameObject Prefab { get { return prefab; } set { prefab = value; } }
    public GameObject ConstructionPrefab { get { return constructionPrefab; } set { constructionPrefab = value; } }

    public Sector Group { get { return this.sector; } set { this.sector = value; } }

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

    public Sector GetSector()
    {
        return sector;
    }

    public void SetColor(int Red, int Green, int Blue)
    {
        this.backgroundColor.r = Red;
        this.backgroundColor.g = Green;
        this.backgroundColor.b = Blue;
        this.backgroundColor.a = 255;
    }

    public Sector ConvertSectorStringData(string data)
    {
        switch (data)
        {
            case "EngineRoom":
                return Sector.Ubikace;
            case "Store":
                return Sector.Sklad;
            case "Dzungle":
                return Sector.Strojovna;
            case "Lesy":
                return Sector.Dilna;
            case "dsad":
                return Sector.Laborator;
            case "dsqqad":
                return Sector.Strilna;
            case "dasd":
                return Sector.Cela;
            case "ddddd":
                return Sector.Garaz;
            case "Loueeeky":
                return Sector.Kaple;
            default:
                return Sector.VolnePole;

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

public enum Sector { Ubikace, Strojovna, Dilna, Sklad, Laborator, Strilna, Cela, Garaz, Kaple, VolnePole};

public enum RawMaterials { bojler, varic, lednicka, usmernovac, sadaNaradi, sverak, kovadlina , rozsireneNaradi, elektrickeRucniNastroje, strojniZarizeni, automatizacniJednotka, Computer , zakladniNastroje,None };
