using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelControler : MonoBehaviour
{

    [SerializeField] GameObject cheatPanel;
    public GameObject panel; // ToDo Pridam vsechny panely a pak vymyslim lepsi mechanismus zavirani a vypinani.
    public bool isActive = false;
    [SerializeField] List<GameObject> panels = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        AddToList();
        panel.SetActive(isActive); // ToDo to souvisi stim prvnim
        cheatPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        KeyControl();
    }


    public void TurnOn()
    {
        isActive = !isActive;
        panel.SetActive(isActive);  // ToDo to souvisi stim prvnim
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
            if(isActive)
            isActive = !isActive;
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

}
