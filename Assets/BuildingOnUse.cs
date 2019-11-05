using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOnUse : MonoBehaviour
{
    private Building currentBuilding;
    private ResourceControler resourceControler;

    private void Awake()
    {
       SetButtonEvent();
    }

    public void CacheBuilding(Building building)
    {
        currentBuilding = building;
    }

    public void CacheResourcesControler(ResourceControler rc)
    {
        resourceControler = rc;
    }

    public void SetButtonEvent ()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => ButtonOnClick());
    }

    public void ButtonOnClick ()
    {
        bool haveEnoughtResources = false;
        haveEnoughtResources =  resourceControler.TryBuildBuilding(currentBuilding);

        if(haveEnoughtResources)
        {
            Debug.Log("Button Executed !!!!  " + this.name);
        }
        
    }



}
