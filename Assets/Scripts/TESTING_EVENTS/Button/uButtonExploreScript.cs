using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uButtonExploreScript : MonoBehaviour
{
    [SerializeField]
    private GameObject parentObject;

    private RegionControler regionControler;

    [SerializeField]
    GameObject panelMission;

    public void YesButton()
    {
        Debug.Log("Yes Button -> Show Mission panel and other shits");
        regionControler.ExploreRegion();
        parentObject.SetActive(false);
    }

    public void NoButton()
    {
        Debug.Log("No Button -> Close Fucking Explores buton");
        parentObject.SetActive(false);
    }

    public void In(RegionControler regionControler)
    {
        this.regionControler = regionControler;
    }

    void OnEnable()
    {
        Debug.Log("me enabled");
    }


    public void SetMissionPanel ()
    {

    }





}
