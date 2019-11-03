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

    private BuildingManager bm;
    [Header("ButtonPrefab")]
    public GameObject buildingPrefab;

    private void Awake()
    {
        bm = FindObjectOfType<BuildingManager>();
        AddAllBuildings();
    }

    public void AddAllBuildings()
    {
        foreach (var item in bm.GetBuildingList())
        {
            AddBuildingHolder(item);
        }
    }

    public void AddBuildingHolder (Building building)
    {
        Sector sector = building.GetSector();

        GameObject go = Instantiate(buildingPrefab);
        ChoiceBuildingHolder(sector,go);
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        BuildingButtonBuilder bbb = go.GetComponent<BuildingButtonBuilder>();
        bbb.BuildingChangeStats(building,bm);
       // bbb.BroadcastMessage("SetBackgroundImage", bm);
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


