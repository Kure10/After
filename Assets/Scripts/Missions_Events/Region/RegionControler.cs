﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionControler : MonoBehaviour
{

    public float fogValue = 0.25f;
    public Color black = new Color(0, 0, 0, 1f);
    public Color idleColor = new Color(1, 1, 1, 1);

    private RegionOperator[] arrayOfregionsOperator;
    private MissionManager missionManager;
    private MissionController missionController;

    //public static List<uButtonExploreScript> ubes = new List<uButtonExploreScript>();
    //public delegate void OnExploreButtonClick();
    //public static event OnExploreButtonClick onClick;

    private void Awake()
    {
        arrayOfregionsOperator = FindObjectsOfType<RegionOperator>();
        missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
        missionController = FindObjectOfType<MissionController>();
    }

    public void ChangeRegionState(Region region, Image regionImage)
    {
        ChangeColoring(region, regionImage);
    }

    public void RefreshRegions()
    {
        foreach (RegionOperator item in arrayOfregionsOperator)
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

    public void StartExploreMision(RegionOperator regionOperator)
    {
        missionManager.ChoiseMission(regionOperator, true);
    }

    public void AskManagerToMission(uButtonAdditionalMission mission, RegionOperator regionOperator)
    {
        missionManager.ChoiseMissionForRegionButton(mission, regionOperator);
    }

    public bool AskControllerIsMissionInProgress(long missionId)
    {
        return missionController.IsMissionInProgress(missionId);
    }

}