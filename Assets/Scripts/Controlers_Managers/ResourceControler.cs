using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceControler : MonoBehaviour
{

    [SerializeField]
    ResourceManager resourceManager;

    // todo -- Tady bude vice podminek. Zatím tady jsou tyhle jenom.
    // Jde o to ze nevím jak budou poreseny Raw Materials a taky jak Elektrina. a dalsí..
    public bool TryBuildBuilding(Building building)
    {
       if(CheckEnoughTechnickyMaterial(building.Tech) && CheckEnoughCivilniMaterial(building.Civil) && CheckEnoughVojenskyMaterial(building.Military))
        {

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
