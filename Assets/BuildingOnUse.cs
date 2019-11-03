using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOnUse : MonoBehaviour
{
    private Building currentBuilding; 


    private void Awake()
    {
       SetButtonEvent();
    }

    public void CacheBuilding(Building building)
    {
        currentBuilding = building;
    }

    public void SetButtonEvent ()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => ButtonOnClick());
    }

    public void ButtonOnClick ()
    {
        Debug.Log("Button Executed !!!!  "  + this.name );
    }


}
