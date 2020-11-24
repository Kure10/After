using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uWindowMissionSpecSelection : MonoBehaviour
{


    [SerializeField] GameObject specPrefab;

    [SerializeField] GameObject holder;

    [SerializeField] Text infoText;

    [Header("Buttons")]
    [SerializeField] Button confirm;

    [SerializeField] Button back;

    [Header("Blocker")]
    [SerializeField] GameObject blocker;

    private bool isWindowActive = false;


    public GameObject SpecPrefab { get { return this.specPrefab; } }

    public GameObject SpecHolder { get { return this.holder; } }

    public Button GetConfirmButton { get { return this.confirm; } }

    public Button GetBackButton { get { return this.back; } }

    public bool IsWindowActive { get { return this.isWindowActive; } }
   

   // public string InfoText { get { return this.infoText.text; } set { this.infoText.text = value; } }

    public void ActivateWindow()
    {
        this.gameObject.SetActive(true);
        this.isWindowActive = true;
    }

    public void DisableWindow()
    {
        this.gameObject.SetActive(false);
        this.isWindowActive = false;
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void setInfoText (int minValue , int maxValue)
    {
        this.infoText.text = $"{minValue} / {maxValue}"; 
    }

    public void ActivateBlocker()
    {
        this.blocker.SetActive(true);
    }

    public void DisableBlocker()
    {
        this.blocker.SetActive(false);
    }
}
