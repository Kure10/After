using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Building", fileName = "Building")]
public class Building : ScriptableObject
{
    [Header("Main Settings")]
    [SerializeField] string buildingName = "Default";
    [SerializeField]
    Sector sector = Sector.ubykace;
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
    [SerializeField] int rawMaterial = 0;
    [Header("Images")]
    [SerializeField] Sprite ilustrationImage;
    [Header("Text Information")]
    [TextArea(4, 10)]
    [SerializeField] string textForInfo = "Lazy GameDesign";


    public string GetName { get { return buildingName; } }
    public int GetCivil { get { return civilniMaterial; } }
    public int GetTech { get { return technickyMaterial; } }
    public int GetMilitary { get { return vojenskyMaterial; } }
    public int GetRawMaterial { get { return rawMaterial; } }
    public Sprite GetSprite { get { return ilustrationImage; } }
    public int GetSize { get { return sizeOfBuilding; } }
    public string GetInfo { get { return textForInfo; } }

    public Sector GetSector()
    {
        return sector;
    }

}

public enum TypeOfBuilding {basis , upgrade , extension};

public enum Sector { ubykace, dilna, strilna, sklad, vezeni, laborator, agregat, garaz, kaple};

