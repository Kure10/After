using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    [SerializeField] private List<BuildingBlueprint> buildings = new List<BuildingBlueprint>();

    [Space]
    [Header("Background Images For Buildings")]
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    [SerializeField] private BuildingController buildingController;

    private Dictionary<Tag, Sprite> backgroundImages = new Dictionary<Tag, Sprite>();

    public List<BuildingBlueprint> GetBuildingList()
    {
        return buildings;
    }

    private void Awake()
    {
        BuildingXmlLoader xmlLoader = new BuildingXmlLoader();
        buildings = xmlLoader.GetBuildingsFromXML();


        SetUpDictionary();

        foreach (var item in buildings)
            buildingController.AddBuildingButton(item, this);
    }

    
    // obsolete Destroy as soon is posible
    private void SetUpDictionary()
    {
        backgroundImages.Add(Tag.Strojovna, sprites[0]);
        backgroundImages.Add(Tag.Dilna, sprites[1]);
        backgroundImages.Add(Tag.Garaz, sprites[2]);
        backgroundImages.Add(Tag.Kaple, sprites[3]);
        backgroundImages.Add(Tag.Laborator, sprites[4]);
        backgroundImages.Add(Tag.Sklad, sprites[5]);
        backgroundImages.Add(Tag.Strilna, sprites[6]);
        backgroundImages.Add(Tag.Ubikace, sprites[7]);
        backgroundImages.Add(Tag.Cela, sprites[8]);
        backgroundImages.Add(Tag.VolnePole, sprites[9]);
    }

    // obsolete Destroy as soon is posible
    public Sprite GetSprite (Tag sector)
    {
        Sprite sprite;
        return sprite = backgroundImages[sector];
    }


}
