using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControler : MonoBehaviour
{
    private GameObject hotkeys;
    void Start()
    {
        hotkeys = GameObject.FindGameObjectWithTag("hotkeys");
        hotkeys.SetActive(false);
    }
    public void ReverseActivity(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void QuitAplication ()
    {
        Debug.Log(" End Is Comming Soon !!!");
        Application.Quit();
    }

    public void OpenHotkeys ()
    {
      hotkeys.SetActive(true);

    }

    public void CloseHotkeys()
    {
        hotkeys.SetActive(false);

    }

}
