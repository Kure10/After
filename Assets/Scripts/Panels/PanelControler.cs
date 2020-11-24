using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelControler : MonoBehaviour
{
    [SerializeField] PanelTime time;
    [SerializeField] GameObject cheatPanel;
    [SerializeField] GameObject map;
    [SerializeField] Button mapButton;
    [SerializeField] List<GameObject> panels = new List<GameObject>();
    [Space]
    [SerializeField] GameObject block;

    // Start is called before the first frame update
    void Start()
    {
        SetActiveAll(false);
        panels[0].transform.parent.gameObject.SetActive(true);

        mapButton.onClick.RemoveAllListeners();
        mapButton.onClick.AddListener(()=> OpenMap());
    }

    // Update is called once per frame
    void Update()
    {
        KeyControl();
    }


    public void TurnOn(int currentPanel)
    {

        if (panels[currentPanel].activeSelf == true)
        {
            time.Pause();
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
                time.Pause();
                block.SetActive(true);
                CameraMovement.MovementAllEnable(false);
                return;
            }
            else
            {
                
                block.SetActive(false);
                CameraMovement.MovementAllEnable(true);
            }
        }
    }


    public void KeyControl ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CameraMovement.MovementAllEnable(true);

            foreach (var item in panels)
            {
                if(item.activeSelf)
                {
                    time.Pause();
                    block.SetActive(false);
                    break;
                }
            }

            if (map.activeSelf)
                map.SetActive(false);


            DisableAllPanels();
        }
    }

    public void DisableAllPanels()
    {
        foreach (var item in panels)
        {
            if (item.activeSelf)
            {
                item.SetActive(false);
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

    public void CloseThisPanel(GameObject gameObject)
    {
        CameraMovement.MovementAllEnable(true);
        gameObject.SetActive(false);
    }

    private void OpenMap()
    {
        if (map.activeSelf)
        {
            map.SetActive(false);
            return;
        }
            
        map.SetActive(true);
    }

}
