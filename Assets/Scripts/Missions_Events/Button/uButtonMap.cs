using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class uButtonMap : MonoBehaviour
{
    [SerializeField] Button mapButton;

    [SerializeField] Text text; // Will be remove after button will be complete.

    public Button GetMapButton { get { return this.mapButton; } }

    public Text GetText { get { return this.text; } }

    public void SwitchButtonView(GameObject map)
    {
        if(map.activeSelf)
        {
            this.text.text = "Bunker";
        }
        else
        {
            this.text.text = "Map";
        }

    }
}
