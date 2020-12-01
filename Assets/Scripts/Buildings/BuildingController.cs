using System.Collections;
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

    private BuildingManager buildingManager;
    [Header("ButtonPrefab")]
    public GameObject buildingPrefab;

    private void Awake()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        AddAllBuildings();
    }

    private void AddAllBuildings()
    {
        foreach (var item in buildingManager.GetBuildingList())
        {
            AddBuildingButton(item);
        }
    }

    private void AddBuildingButton(BuildingBlueprint buildingBlueprint)
    {
        Sector sector = buildingBlueprint.GetSector();

        GameObject go = Instantiate(buildingPrefab);
        ChoiceBuildingHolder(sector, go);
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        BuildingButtonBuilder bbb = go.GetComponent<BuildingButtonBuilder>();
        bbb.BuildingChangeStats(buildingBlueprint, buildingManager);
    }


    // this method is broken. And obsolete...
    private void ChoiceBuildingHolder(Sector sector, GameObject go)
    {
        switch (sector)
        {
            case Sector.Cela:
                go.transform.parent = agregat.transform;
                break;
            case Sector.Strojovna:
                go.transform.parent = dilna.transform;
                break;
            case Sector.Garaz:
                go.transform.parent = garaz.transform;
                break;
            case Sector.Kaple:
                go.transform.parent = kaple.transform;
                break;
            case Sector.Strilna:
                go.transform.parent = laborator.transform;
                break;
            case Sector.Sklad:
                go.transform.parent = sklad.transform;
                break;
            case Sector.Dilna:
                go.transform.parent = strilna.transform;
                break;
            case Sector.Ubikace:
                go.transform.parent = ubikace.transform;
                break;
            case Sector.Laborator:
                go.transform.parent = vezeni.transform;
                break;
            case Sector.VolnePole:
                go.transform.parent = vezeni.transform;
                break;
            default:
                Debug.Log("Building has not button Holder -> Error");
                break;
        }
    }

}
