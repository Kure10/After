using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControler : MonoBehaviour
{
    [SerializeField]
    GameObject hotKeys; // jina možnost me nenapada
   // private GameObject hotkeys;
    void Start()
    {
        // hotkeys = GameObject.FindObjectOfType<Menu>().gameObject;
        // menu = GameObject.FindGameObjectWithTag("hotkeys");
        hotKeys.SetActive(false);
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
        hotKeys.SetActive(true);

    }

    public void CloseHotkeys()
    {
        hotKeys.SetActive(false);

    }

}
