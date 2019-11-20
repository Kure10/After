﻿using System.Collections;
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
        return CheckMaterialAvailability(building.Tech, ResourceManager.Material.Technicky) &&
               CheckMaterialAvailability(building.Civil, ResourceManager.Material.Civilni) &&
               CheckMaterialAvailability(building.Military, ResourceManager.Material.Vojensky);
        /*
if(CheckEnoughTechnickyMaterial(building.Tech) && CheckEnoughCivilniMaterial(building.Civil) 
&& CheckEnoughVojenskyMaterial(building.Military))
 {

     // Allow Build fucking building;
     return true;
 }*/
    }

    private bool CheckMaterialAvailability(int value, ResourceManager.Material type)
    {
        return resourceManager.GetResourceCount(type) >= value;
    }
}