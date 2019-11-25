using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uButtonScript : MonoBehaviour
{
    [SerializeField]
    private GameObject parentObject;

    public void YesButton()
    {
        Debug.Log("Yes Button -> Show Mission panel and other shits");
        parentObject.SetActive(false);
    }

    public void NoButton()
    {
        Debug.Log("No Button -> Close Fucking Explores buton");
        parentObject.SetActive(false);
    }



}
