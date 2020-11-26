using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuControler : MonoBehaviour
{
    [SerializeField] PanelTime time;
    [Header("Buttons")]
    [SerializeField] Button menuButton;
    [SerializeField] Button backButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button hotKeysButton;
    [Header("Panels")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject hotKeys;
    [SerializeField] GameObject blockPanel;

    static public bool isInMenu = false;

    public bool IsMenuPanelActive { get { return menuPanel.activeSelf; } }

    private void Awake()
    {
        hotKeysButton.onClick.RemoveAllListeners();
        hotKeysButton.onClick.AddListener(() => OpenHotkeys());
        menuButton.onClick.RemoveAllListeners();
        menuButton.onClick.AddListener(() => ActivateMenu());
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => DisableMenu());
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(() => QuitAplication());

        hotKeys.SetActive(false);
        blockPanel.SetActive(false);
    }

    public void DisableMenu()
    {
        time.UnpauseGame(fromMenu: true);
        menuPanel.SetActive(false);
        blockPanel.SetActive(false); 
    }

    public void ActivateMenu()
    {
        time.PauseGame(intoMenu: true);
        menuPanel.SetActive(true);
        blockPanel.SetActive(true);
    }

    public void QuitAplication ()
    {
        Debug.Log(" End Is Comming Soon !!!");
        Application.Quit();
    }

    public void OpenHotkeys ()
    {
        hotKeys.SetActive(true);
        hotKeysButton.onClick.RemoveAllListeners();
        hotKeysButton.onClick.AddListener(() => CloseHotkeys());

    }

    public void CloseHotkeys()
    {
        hotKeys.SetActive(false);
        hotKeysButton.onClick.RemoveAllListeners();
        hotKeysButton.onClick.AddListener(() => OpenHotkeys());

    }

}
