using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    [SerializeField]
    private List<Building> buildings = new List<Building>();

    [Space]
    [Header("Background Images For Buildings")]
    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    private Dictionary<Sector, Sprite> backgroundImages = new Dictionary<Sector, Sprite>();

    public List<Building> GetBuildingList()
    {
        return buildings;
    }

    private void Awake()
    {
        SetUpDictionary();
    }

    
    private void SetUpDictionary()
    {
        backgroundImages.Add(Sector.agregat, sprites[0]);
        backgroundImages.Add(Sector.dilna, sprites[1]);
        backgroundImages.Add(Sector.garaz, sprites[2]);
        backgroundImages.Add(Sector.kaple, sprites[3]);
        backgroundImages.Add(Sector.laborator, sprites[4]);
        backgroundImages.Add(Sector.sklad, sprites[5]);
        backgroundImages.Add(Sector.strilna, sprites[6]);
        backgroundImages.Add(Sector.ubikace, sprites[7]);
        backgroundImages.Add(Sector.vezeni, sprites[8]);
    }

    public Sprite GetSprite (Sector sector)
    {
        Sprite sprite;
        return sprite = backgroundImages[sector];
    }


}
