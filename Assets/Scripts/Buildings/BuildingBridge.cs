﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBridge : MonoBehaviour
{
    [SerializeField]
    GameObject ubikace;
    [SerializeField]
    GameObject dilna;
    [SerializeField]
    GameObject sklad;

    private BuildingControler bc;

    public GameObject buildingPrefab;

    private void Awake()
    {
        bc = FindObjectOfType<BuildingControler>();
    }

    private void Start()
    {
        AddAllBuildings();
    }

    public void AddBuildingHolder (Building building)
    {
        GameObject ga = Instantiate(buildingPrefab);
        if(building.GetVariety() == Variety.dilna)
        {
            ga.transform.parent = dilna.transform;
        }
        else if (building.GetVariety() == Variety.ubykace)
        {
            ga.transform.parent = ubikace.transform;
        }
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        BuildingBuilder bb = ga.GetComponent<BuildingBuilder>();
        bb.BuildingChangeStats(building);

    }

    public void AddAllBuildings()
    {
        foreach (var item in bc.GetBuildingList())
        {
            AddBuildingHolder(item);
        }
    }

}
