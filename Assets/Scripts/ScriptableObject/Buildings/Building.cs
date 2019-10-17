using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Building", fileName = "Building")]
public class Building : ScriptableObject
{
    [Header("Main Settings")]
    [SerializeField] string buildingName = "Default";
    [SerializeField]
    TypeOfBuilding type = TypeOfBuilding.basis;
    [Tooltip("v sekundach")]
    [SerializeField] float timeToBuild = 20f;
    
    [Header("Size of Building")]
    [Range(1, 6)]
    public int sizeOfBuilding = 1;

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

}

enum TypeOfBuilding {basis , upgrade , extension};
