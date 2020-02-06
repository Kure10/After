﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionControler : MonoBehaviour
{

    public float fogValue = 0.25f;
    public Color black = new Color(0, 0, 0, 1f);
    public Color idleColor = new Color(1, 1, 1, 1);

    private RegionSettings[] arrayOfregionsSettings;
    private MissionManager missionManager;

    

    //public static List<uButtonExploreScript> ubes = new List<uButtonExploreScript>();
    //public delegate void OnExploreButtonClick();
    //public static event OnExploreButtonClick onClick;

    private void Awake()
    {
        arrayOfregionsSettings = FindObjectsOfType<RegionSettings>();
        missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //public static void OnReginsterOpenEvent(uButtonExploreScript tmp)
    //{
    //    ubes.Add(tmp);
    //}

    //public static void UnReginsterOpenEvent(uButtonExploreScript tmp)
    //{
    //    if(ubes.Contains(tmp))
    //        ubes.Remove(tmp);
    //}

    public void ChangeRegionState(Region region, Image regionImage)
    {
        ChangeColoring(region, regionImage);
    }

    public void RefreshRegions()
    {
        foreach (var item in arrayOfregionsSettings)
        {
            var regionImage = item.GetImage();
            var region = item.GetRegion();
            ChangeColoring(region, regionImage);
        }
    }

    private void ChangeColoring(Region region, Image regionImage)
    {
        if (region.IsExplored)
        {
            regionImage.color = idleColor;
        }
        else if (region.IsInDarkness)
        {
            regionImage.color = black;
        }
        else if (region.IsInShadow)
        {
            regionImage.color = new Color(1, 1, 1, fogValue);
        }
    }

    public void StartExploreMision()
    {
        missionManager.ChoiseMission();
    }

}
