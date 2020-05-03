using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uWindowMissionNotification : MonoBehaviour
{
    [SerializeField]
    private Text tittle;

    [SerializeField]
    private Text subTittle;

    [SerializeField]
    private Text buttonOneText;

    [SerializeField]
    private Text buttonTwoText;

    [Header("Buttons")]

    [SerializeField]
    private Button buttonOne;

    [SerializeField]
    private Button buttonTwo;

    /*Tady toho muze byt vice.  To se jeste domluvím co by vsechno měla obsahovat notifikace..*/

    #region Properity

    public Text Tittle { get { return tittle; } set { tittle = value; } }
    public Text SubTittle { get { return subTittle; } set { subTittle = value; } }
    public Text ButtonOneText { get { return buttonOneText; } set { buttonOneText = value; } }
    public Text ButtonTwoText { get { return buttonTwoText; } set { buttonTwoText = value; } }
    public Button ButtonOne { get { return buttonOne; } set { buttonOne = value; } }
    public Button ButtonTwo { get { return buttonTwo; } set { buttonTwo = value; } }

    #endregion

}
