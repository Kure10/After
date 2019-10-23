using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControler : MonoBehaviour
{

    [SerializeField]
    private List<Building> buildings = new List<Building>();

    [SerializeField]
    public List<Sprite> sprites = new List<Sprite>();

    // zkouska
    public Dictionary<Sector, Sprite> backgroundImages = new Dictionary<Sector, Sprite>();

    // Tady bych asi vymyslel nejake ID budov. Aby se to nemuselo vybirat podle jmena. , Nebo nejaky jiny identifikator.
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


    // zkouska
    public void FullFillDictionary()
    {
        Debug.Log(System.Enum.GetValues(typeof(Sector)).Length);

        backgroundImages.Add(Sector.agregat, sprites[0]);
        backgroundImages.Add(Sector.dilna, sprites[1]);
        backgroundImages.Add(Sector.garaz, sprites[2]);
        backgroundImages.Add(Sector.kaple, sprites[3]);
        backgroundImages.Add(Sector.laborator, sprites[4]);
        backgroundImages.Add(Sector.sklad, sprites[5]);
        backgroundImages.Add(Sector.strilna, sprites[6]);
        backgroundImages.Add(Sector.ubykace, sprites[7]);
        backgroundImages.Add(Sector.vezeni, sprites[8]);
    }

}
