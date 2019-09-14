using System.Collections;
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
        AddToList();
        SetActiveAll(false);
    }

    // Update is called once per frame
    void Update()
    {
        KeyControl();
    }


    public void TurnOn(int currentPanel)
    {
        
        if(panels[currentPanel].activeSelf == true)
        {
            panels[currentPanel].SetActive(false);
        }
        else
        {
            SetActiveAll(false);
            panels[currentPanel].SetActive(true);
        }
    }


    public void KeyControl ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (var item in panels)
            {
                if (item.activeSelf)
                {
                    item.SetActive(false);
                }
            }
        }
    }

    public void AddToList()
    {
        foreach (GameObject items in GameObject.FindGameObjectsWithTag("ExtensionPanel"))
        {
            panels.Add(items);
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
