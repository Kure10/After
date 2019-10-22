using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControler : MonoBehaviour
{

    [SerializeField]
    private List<Building> buildings = new List<Building>();

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

}
