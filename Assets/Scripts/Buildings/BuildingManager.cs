using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    [SerializeField] private List<BuildingBlueprint> buildings = new List<BuildingBlueprint>();

    [Space] [Header("Background Images For Buildings")] [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    /* -- presunuto do blueprints
     [Space] [Header("Construction prefabs 1x1,2x1,2x2,2x3")] [SerializeField]
    private List<GameObject> construcion = new List<GameObject>(); */

    private Dictionary<Sector, Sprite> backgroundImages = new Dictionary<Sector, Sprite>();

    public List<BuildingBlueprint> GetBuildingList()
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
