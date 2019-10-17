﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelControler : MonoBehaviour
{

    [SerializeField] GameObject cheatPanel;
    [SerializeField] List<GameObject> panels = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetActiveAll(false);
    }

    // Update is called once per frame
    void Update()
    {
        KeyControl();
    }


    public void TurnOn(int currentPanel)
    {
        //CameraMovement.scrollEnabled = true;

        if (panels[currentPanel].activeSelf == true)
        {
            panels[currentPanel].SetActive(false);
        }
        else
        {
            SetActiveAll(false);
            panels[currentPanel].SetActive(true);
        }


        foreach (var item in panels)
        {
            if(item.activeSelf == true)
            {
                CameraMovement.scrollEnabled = false;
                return;
            }
            else
            {
                CameraMovement.scrollEnabled = true;
            }
        }
    }


    public void KeyControl ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CameraMovement.scrollEnabled = true;

            foreach (var item in panels)
            {
                if (item.activeSelf)
                {
                    item.SetActive(false);
                }
            }
        }
    }

    public void ShowCheats()
    {
        cheatPanel.SetActive(!cheatPanel.activeSelf);
    }

    public void SetActiveAll(bool status)
    {
        cheatPanel.SetActive(status);
        foreach (var item in panels)
        {
            item.SetActive(status);
        }
    }



}
