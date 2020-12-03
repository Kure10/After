using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class uButtonMap : MonoBehaviour
{
    [SerializeField] Button mapButton;

    [SerializeField] GameObject mapView;

    [SerializeField] GameObject bunkerView;


    public Button GetMapButton { get { return this.mapButton; } }

    public void SwitchButtonView(GameObject map)
    {
        ToggleButtonView(map.activeSelf);
    }

    private void ToggleButtonView(bool isMapActive)
    {
        if(isMapActive)
        {
            bunkerView.SetActive(true);
            mapView.SetActive(false);
        }
        else
        {
            bunkerView.SetActive(false);
            mapView.SetActive(true);
        }
    }
}
