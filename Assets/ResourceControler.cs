using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceControler : MonoBehaviour
{

    [SerializeField]
    ResourceManager resourceManager;

    // todo -- Musim to dodelat
    public bool TryBuildBuilding(Building building)
    {
       if(CheckEnoughTechnickyMaterial(building.Tech) && CheckEnoughCivilniMaterial(building.Civil) && CheckEnoughVojenskyMaterial(building.Military))
        {

            resourceManager.CivilniMaterial -= building.Civil;
            resourceManager.TechnickyMaterial -= building.Tech;
            resourceManager.VojenskyMaterial -= building.Military;
            // todo  and others

            // Allow Build fucking building;
            return true;
        }

        return false;
    }


    private bool CheckEnoughTechnickyMaterial(int value)
    {
        if (resourceManager.TechnickyMaterial >= value)
            return true;
         
        return false;
    }      

    private bool CheckEnoughVojenskyMaterial (int value)
    {
         if (resourceManager.VojenskyMaterial >= value)
            return true;

        return false;
    }

     private bool CheckEnoughCivilniMaterial(int value)
     {
        if (resourceManager.CivilniMaterial >= value)
            return true;

        return false;
     }
            
}
