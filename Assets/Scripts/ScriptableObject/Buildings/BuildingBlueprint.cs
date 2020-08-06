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
    Sector sector = Sector.ubikace;
    [SerializeField]
    TypeOfBuilding type = TypeOfBuilding.basis;
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

    public void SetSector(Sector sec)
    {
        sector = sec;
    }

    public TypeOfBuilding GetTypeOfBuilding()
    {
        return type;
    }

    public void SetTypeOfBuilding(TypeOfBuilding tob)
    {
        type = tob;
    }

}

public enum TypeOfBuilding {basis , upgrade , extension};

public enum Sector { ubikace, dilna, strilna, sklad, vezeni, laborator, agregat, garaz, kaple};

public enum RawMaterials { bojler, varic, lednicka, usmernovac, sadaNaradi, sverak, kovadlina , rozsireneNaradi, elektrickeRucniNastroje, strojniZarizeni, automatizacniJednotka, Computer , zakladniNastroje,None };
