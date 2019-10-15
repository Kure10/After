using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControler : MonoBehaviour
{

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
        Debug.Log("Hotkeays opend !");
    }

}
