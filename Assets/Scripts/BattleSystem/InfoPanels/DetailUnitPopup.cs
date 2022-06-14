using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailUnitPopup : MonoBehaviour
{

    public void ShowPopup(Unit unit)
    {
        Debug.Log(unit.GetName);
        this.gameObject.SetActive(true);
    }




}
