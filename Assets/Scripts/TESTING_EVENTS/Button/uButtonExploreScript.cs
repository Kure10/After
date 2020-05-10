using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uButtonExploreScript : MonoBehaviour
{
    [SerializeField]
    private GameObject parentObject;

    [SerializeField]
    private RegionSettings regionSettings;

    

    private void Awake()
    {
       
    }

    public void YesButton()
    {
        regionSettings.StartExploreMission();
        //Debug.Log("Yes Button -> Show Mission panel and other shits");
      //  regionSettings.ExploreRegion();
        parentObject.SetActive(false);
    }

    public void NoButton()
    {
        //Debug.Log("No Button -> Close Fucking Explores buton");
        parentObject.SetActive(false);
    }


    void OnEnable()
    {
      //  RegionControler.OnReginsterOpenEvent(this);
    }

    private void OnDisable()
    {
      //  RegionControler.UnReginsterOpenEvent(this);
    }


}
