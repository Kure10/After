﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    [SerializeField] GameObject ubikace;
    [SerializeField] GameObject dilna;
    [SerializeField] GameObject sklad;
    [SerializeField] GameObject strojovna;
    [SerializeField] GameObject strilna;
    [SerializeField] GameObject vezeni;
    [SerializeField] GameObject laborator;
    [SerializeField] GameObject agregat;
    [SerializeField] GameObject garaz;
    [SerializeField] GameObject kaple;

    
    [Header("ButtonPrefab")]
    public GameObject buildingPrefab;

    public void AddBuildingButton(BuildingBlueprint buildingBlueprint, BuildingManager buildingManager)
    {
        Tag sector = buildingBlueprint.GetTag();

        GameObject go = Instantiate(buildingPrefab);
        ChoiceBuildingHolder(sector, go);
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        BuildingButton bbb = go.GetComponent<BuildingButton>();
        bbb.BuildingChangeStats(buildingBlueprint, buildingManager);
    }


    // this method is broken. And obsolete...
    private void ChoiceBuildingHolder(Tag sector, GameObject go)
    {
        switch (sector)
        {
            case Tag.Cela:
                go.transform.parent = vezeni.transform;
                break;
            case Tag.Strojovna:
                go.transform.parent = strojovna.transform;
                break;
            case Tag.Garaz:
                go.transform.parent = garaz.transform;
                break;
            case Tag.Kaple:
                go.transform.parent = kaple.transform;
                break;
            case Tag.Strilna:
                go.transform.parent = strilna.transform;
                break;
            case Tag.Sklad:
                go.transform.parent = sklad.transform;
                break;
            case Tag.Dilna:
                go.transform.parent = dilna.transform;
                break;
            case Tag.Ubikace:
                go.transform.parent = ubikace.transform;
                break;
            case Tag.Laborator:
                go.transform.parent = laborator.transform;
                break;
            case Tag.VolnePole: // agreat pokecat s Nefem..
                go.transform.parent = agregat.transform;
                break;
            default:
                Debug.Log("Building has not button Holder -> Error");
                break;
        }
    }

}
