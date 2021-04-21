using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailUnitPopup : MonoBehaviour
{

    public void ShowPopup(Unit unit)
    {
        Debug.Log(unit._name);
        this.gameObject.SetActive(true);
    }




}
