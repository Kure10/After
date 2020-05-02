using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uWindowNotification : MonoBehaviour
{
    [SerializeField]
    private Text tittle;

    [SerializeField]
    private Text subTittle;

    [SerializeField]
    private Text buttonOne;

    [SerializeField]
    private Text buttonTwo;

    /*Tady toho muze byt vice.  To se jeste domluvím co by vsechno měla obsahovat notifikace..*/

    #region Properity

    public Text Tittle { get { return tittle; } set { tittle = value; } }
    public Text SubTittle { get { return subTittle; } set { subTittle = value; } }
    public Text ButtonOne { get { return buttonOne; } set { buttonOne = value; } }
    public Text ButtonTwo { get { return buttonTwo; } set { buttonTwo = value; } }

    #endregion

}
