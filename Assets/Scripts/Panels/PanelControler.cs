using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelControler : MonoBehaviour
{

    public GameObject panel;
    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(isActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TurnOn()
    {
        isActive = !isActive;
        panel.SetActive(isActive);
    }

}
