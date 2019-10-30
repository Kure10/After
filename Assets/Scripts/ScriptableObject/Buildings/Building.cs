using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Building", fileName = "Building")]
public class Building : ScriptableObject
{
    [Header("Main Settings")]
    [SerializeField] string buildingName = "Default";
    [SerializeField]
    Sector sector = Sector.ubikace;
    [SerializeField]
    TypeOfBuilding type = TypeOfBuilding.basis;
    [Tooltip("v sekundach")]
    [SerializeField] float timeToBuild = 20f;
    
    [Header("Size of Building")]
    [SerializeField]
    [Range(1, 6)]
    private int sizeOfBuilding = 1;

    [Header("Resources cost")]
    [SerializeField] int civilniMaterial = 0;
    [SerializeField] int technickyMaterial = 0;
    [SerializeField] int vojenskyMaterial = 0;
    [Space]
    [SerializeField] private int rawMaterial = 0; // Tohle bude bool -> Když ano tak by se meli zobrazit jeden raw material a tlacitko plus na pridani dalsiho
    [Header("Images")]
    [SerializeField] Sprite ilustrationImage;
    [Header("Text Information")]
    [TextArea(4, 10)]
    [SerializeField] string textForInfo = "Lazy GameDesign";

    public List<RawMaterials> myList = new List<RawMaterials>();


    public string Name { get { return buildingName; } set { buildingName = value; } }
    public int Civil { get { return civilniMaterial; } set { civilniMaterial = value; } }
    public int Tech { get { return technickyMaterial; } set { technickyMaterial = value; } }
    public int Military { get { return vojenskyMaterial; } set { vojenskyMaterial = value; } }
    public int RawMaterial { get { return rawMaterial; } set { rawMaterial = value; } }
    public Sprite Sprite { get { return ilustrationImage; } set { ilustrationImage = value; } }
    public int Size { get { return sizeOfBuilding; } set { sizeOfBuilding = value; } }
    public string Info { get { return textForInfo; } set { textForInfo = value; } }
    public float TimeToBuild { get { return timeToBuild; } set { timeToBuild = value; } }

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

public enum RawMaterials { radiator, dynamit, fyaloveHovinko, peceneKure, BarbekiuOmacka, MojeNohy, None };

