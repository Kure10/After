using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOnUse : MonoBehaviour
{
    private BuildingBlueprint currentBuildingBlueprint;
    private ResourceControler resourceControler;
    private BuildingCreator buildingCreator;
    private PanelControler panelControler;

    private void Awake()
    {
        SetButtonEvent();
        buildingCreator = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<BuildingCreator>();
        panelControler = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PanelControler>();
    }


    public void CacheBuilding(BuildingBlueprint buildingBlueprint)
    {
        currentBuildingBlueprint = buildingBlueprint;
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
        
            if (currentBuildingBlueprint.Prefab != null)
            {
                buildingCreator.CreateBuilding(currentBuildingBlueprint);
                panelControler.DisableAllPanels(); // disable all open panels. Pak tu dam current. Kdyby se nejak rozsirila hra.
                CameraMovement.MovementAllEnable(true);
            }
            else
            {
                Debug.Log("Chybi Prefab pro building!");
            }
            Debug.Log("Button Executed !!!!  " + this.name);
    }
}