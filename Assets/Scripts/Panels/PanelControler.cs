using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelControler : MonoBehaviour
{

    public GameObject panel;
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TurnOn()
    {
        if (isActive == false)
        {
            panel.SetActive(true);
            isActive = !isActive;
        }
        else
        {
            panel.SetActive(false);
            isActive = !isActive;
        }

    }

}
