using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelControler : MonoBehaviour
{
    [SerializeField] PanelTime time;
    [SerializeField] GameObject cheatPanel;
    [SerializeField] MenuControler menuControler;
    [SerializeField] EventController eventControler;
    [SerializeField] WindowMissionController missionWindowControler;
    [SerializeField] GameObject missionShowMissionPanel;
    [SerializeField] GameObject map;
    [SerializeField] uButtonMap uButtonMap;
    [SerializeField] List<GameObject> panels = new List<GameObject>();
    [Space]
    [SerializeField] GameObject blocker;

    public event Action<GameObject> CheckIfMapIsActive = delegate { };

    static public bool isPopupOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        SetActiveAll(false);
        panels[0].transform.parent.gameObject.SetActive(true);

        CheckIfMapIsActive += uButtonMap.SwitchButtonView;

        uButtonMap.GetMapButton.onClick.RemoveAllListeners();
        uButtonMap.GetMapButton.onClick.AddListener(() => OpenMap());

        CheckIfMapIsActive.Invoke(map);
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
            time.UnpauseGame(fromPopup: true);
            panels[currentPanel].SetActive(false);
        }
        else
        {
            time.PauseGame(intoPopup: true);
            SetActiveAll(false);
            panels[currentPanel].SetActive(true);
        }


        foreach (var item in panels)
        {
            if (item.activeSelf == true)
            {
                blocker.SetActive(true);
                CameraMovement.MovementAllEnable(false);
                return;
            }
            else
            {
                
                blocker.SetActive(false);
                CameraMovement.MovementAllEnable(true);
            }
        }
    }



    public void KeyControl ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CameraMovement.MovementAllEnable(true);

            if (EventController.isEventRunning && eventControler.GetEventPanel.activeSelf)
            {
                eventControler.Minimaze();
                return;
            }

            if (menuControler.IsMenuPanelActive)
            {
                menuControler.DisableMenu();
                return;
            }

            if (missionShowMissionPanel.activeSelf)
            {
                missionWindowControler.DisableShowMissionPanel();
                return;
            }

            if (IsPopupActive())
                return;


            if (map.activeSelf)
            {
                map.SetActive(false);
                CheckIfMapIsActive.Invoke(map);
                return;
            }

            DisableAllPanels();

            if (!menuControler.IsMenuPanelActive)
            {
                menuControler.ActivateMenu();
                return;
            }
        }
    }

    private bool IsPopupActive()
    {
        bool isActive = false;

        for (int i = 0; i < panels.Count; i++)
        {
            GameObject item = panels[i];

            if (item.activeSelf)
            {
                TurnOn(i);
                isActive = true;
            }
        }

        return isActive;
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
        blocker.SetActive(false);

        if (map.activeSelf)
        {
            map.SetActive(false);
            CheckIfMapIsActive.Invoke(map);
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
            CheckIfMapIsActive.Invoke(map);
            return;
        }

        map.SetActive(true);

        CheckIfMapIsActive.Invoke(map);    
    }

}
