using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    enum BuildingState
    {
        Designed,
        UnderConstruction,
        Build
        //upgrading? destroyed? - pro kazdy stav by mela byt vlastni grafika
    };

    private BuildingState state;

    private BuildingState State
    {
        get => state;
        set { 
            state = value;
            OnStateChanged();
        }
    }

    private readonly BuildingBlueprint blueprint;
    private GameObject prefab;
  
    public Building(BuildingBlueprint blueprint, GameObject prefab)
    {
        this.blueprint = blueprint;
        this.prefab = prefab; //this is ugly hack just to get selected position easily- the prefab is reInstantiated later
        State = BuildingState.Designed;
    }

    private void OnStateChanged()
    {
        if (prefab != null)
        {
            Object.Destroy(prefab);
        }
        switch (state)
        {
            case BuildingState.Designed: prefab = Object.Instantiate(blueprint.Prefab, prefab.transform.position, prefab.transform.rotation);
                break;
            default: break; //TODO
        }
    }
}