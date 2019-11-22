using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class USettingsMapButton : MonoBehaviour
{
    [SerializeField] Button buttonExplore;






    private void RegisterEvent ()
    {
        buttonExplore.onClick.AddListener(neco);
    }

    private void neco ()
    {

    }

}
