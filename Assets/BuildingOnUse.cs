using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOnUse : MonoBehaviour
{
    private Building currentBuilding;
    private ResourceControler resourceControler;
    private BuildingCreator bc;

    private void Awake()
    {
        SetButtonEvent();
        bc = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildingCreator>();
    }

    public void CacheBuilding(Building building)
    {
        currentBuilding = building;
    }

    public void CacheResourcesControler(ResourceControler rc)
    {
        resourceControler = rc;
    }

    public void SetButtonEvent()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => ButtonOnClick());
    }

    public void ButtonOnClick()
    {
        bool haveEnoughtResources = false;
        haveEnoughtResources = resourceControler.TryBuildBuilding(currentBuilding);

        // Zkontroluje jenom jestli ma dost na počatešní postaveni budovy.. (Nic se neodečíta.)
        if (haveEnoughtResources)
        {
            if (currentBuilding.Prefab != null)
            {
                bc.CreateBuilding(currentBuilding.Prefab);
            }
            else
            {
                Debug.Log("Chybi Prefab pro building!");
            }
            Debug.Log("Button Executed !!!!  " + this.name);
        }

    }



}