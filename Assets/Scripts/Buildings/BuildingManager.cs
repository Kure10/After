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

    // Tady bych asi vymyslel nejake ID budov. Aby se to nemuselo vybirat podle jmena. , Nebo nejaky jiny identifikator.
    // podobnou metodu mas i v BuildingBuilderu. 
    public Building GetBuildingByName (string name)
    {
        foreach (var item in buildings)
        {
            if(item.name == name)
            {
                return item;
            }
        }

        return null;
    }

    public List<Building> GetBuildingList()
    {
        return buildings;
    }

    private void Awake()
    {
        SetUpDictionary();
    }

    
    public void SetUpDictionary()
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
        sprite = backgroundImages[sector];
        return sprite;
    }


}
