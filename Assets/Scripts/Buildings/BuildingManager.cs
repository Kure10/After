using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    [SerializeField] private List<BuildingBlueprint> buildings = new List<BuildingBlueprint>();

    [Space]
    [Header("Background Images For Buildings")]
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

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
        //BuildingXmlLoader xmlLoader = new BuildingXmlLoader();
        //buildings = xmlLoader.GetBuildingsFromXML();


        SetUpDictionary();
    }

    
    // obsolete Destroy as soon is posible
    private void SetUpDictionary()
    {
        backgroundImages.Add(Sector.Cela, sprites[0]);
        backgroundImages.Add(Sector.Strojovna, sprites[1]);
        backgroundImages.Add(Sector.Garaz, sprites[2]);
        backgroundImages.Add(Sector.Kaple, sprites[3]);
        backgroundImages.Add(Sector.Strilna, sprites[4]);
        backgroundImages.Add(Sector.Sklad, sprites[5]);
        backgroundImages.Add(Sector.Dilna, sprites[6]);
        backgroundImages.Add(Sector.Ubikace, sprites[7]);
        backgroundImages.Add(Sector.Laborator, sprites[8]);
    }

    // obsolete Destroy as soon is posible
    public Sprite GetSprite (Sector sector)
    {
        Sprite sprite;
        return sprite = backgroundImages[sector];
    }


}
