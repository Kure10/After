using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBridge : MonoBehaviour
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

    private ResourceControler resourceControler;
    private BuildingManager buildingManager;
    [Header("ButtonPrefab")]
    public GameObject buildingPrefab;

    private void Awake()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        resourceControler = FindObjectOfType<ResourceControler>();
        AddAllBuildings();
    }

    private void AddAllBuildings()
    {
        foreach (var item in buildingManager.GetBuildingList())
        {
            AddBuildingHolder(item);
        }
    }

    private void AddBuildingHolder (BuildingBlueprint buildingBlueprint)
    {
        Sector sector = buildingBlueprint.GetSector();

        GameObject go = Instantiate(buildingPrefab);
        ChoiceBuildingHolder(sector,go);
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        BuildingButtonBuilder bbb = go.GetComponent<BuildingButtonBuilder>();
        bbb.BuildingChangeStats(buildingBlueprint,buildingManager);
        BuildingOnUse bou = go.GetComponent<BuildingOnUse>();
        bou.CacheResourcesControler(resourceControler);
    }

    private void ChoiceBuildingHolder (Sector sector, GameObject go)
    {
        switch (sector)
        {
            case Sector.agregat:
                go.transform.parent = agregat.transform;
                break;
            case Sector.dilna:
                go.transform.parent = dilna.transform;
                break;
            case Sector.garaz:
                go.transform.parent = garaz.transform;
                break;
            case Sector.kaple:
                go.transform.parent = kaple.transform;
                break;
            case Sector.laborator:
                go.transform.parent = laborator.transform;
                break;
            case Sector.sklad:
                go.transform.parent = sklad.transform;
                break;
            case Sector.strilna:
                go.transform.parent = strilna.transform;
                break;
            case Sector.ubikace:
                go.transform.parent = ubikace.transform;
                break;
            case Sector.vezeni:
                go.transform.parent = vezeni.transform;
                break;
            default:
                Debug.Log("Building has not button Holder -> Error");
                break;
        }
    }
}


