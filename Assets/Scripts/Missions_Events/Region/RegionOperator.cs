﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionOperator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Region region;
    [SerializeField] string regionIdentifikator;
    [SerializeField] List<uButtonAdditionalMission> uButtAdditionalMission = new List<uButtonAdditionalMission>();

    private RegionControler regionControler;

    private Image image;

    #region Properities

    public Region Region { get { return this.region; } set { this.region = value; } }

    public string RegionIdentifikator { get { return this.regionIdentifikator; } set { this.regionIdentifikator = value; } }

    #endregion

    private void Awake()
    {
        this.regionControler = GameObject.FindObjectOfType<RegionControler>();
    }


    public void ExploreRegion()
    {
        if (this.region.IsInShadow)
        {
            this.region.IsExplored = true;
            this.region.RevealNeighbors();
            this.ActivateAdditionalMissions(true);
            regionControler.RefreshRegions();
        }
    }

    public void CompleteMission(bool isRepeate , long missionID , bool unlockNeighborhodRegions = false)
    {
        // is repeate budu potrebovat na to abych schoval tlacitko nebo ukazal
        if(unlockNeighborhodRegions)
        {
            foreach (Region item in region.neighborhoodRegions)
            {
                item.MissCompReq -= 1;
            }
        }

        if (isRepeate)
        {
            foreach (uButtonAdditionalMission item in this.uButtAdditionalMission)
            {
                if (item.CurrentMission.Id == missionID)
                    item.TemporarilyInactive(true);
            }

        }
        else
        {
            foreach (uButtonAdditionalMission item in this.uButtAdditionalMission)
            {
                if (item.CurrentMission.Id == missionID)
                    item.MissionWasCompleted();
            }
        }

        //  regionControler.RefreshRegions();
        
    }

    public void RefreshMissionButton(Mission mission)
    {
        foreach (uButtonAdditionalMission item in this.uButtAdditionalMission)
        {
            if (item.CurrentMission.Id == mission.Id)
                item.TemporarilyInactive(false);

        }
    }

    public void InicializationRegion()
    {
        this.image = GetComponent<Image>();
        image.sprite = this.region.Sprite;
        image.color = new Color(255, 255, 255);
        this.name = this.region.regionName;
        if (this.region.IsStartingRegion)
        {
            this.region.IsInShadow = true;
        }
        else
        {
            this.region.IsInDarkness = true;
        }
        this.ActivateAdditionalMissions(false);
        regionControler.ChangeRegionState(this.region, this.image);
    }

    public void OpenExplorePanel()
    {
        if (this.region.IsExplored || this.region.IsInDarkness)
            return;

        if (regionControler.AskControllerIsMissionInProgress(region.MapArena))
        {
            // misse je in progrss..... Nedelej nic dvakrat stejna misse in progress
            // nejaky warning
            return;
        }

        regionControler.StartExploreMision(this);

    }

    private void ActivateAdditionalMissions(bool activate)
    {

        foreach (uButtonAdditionalMission item in this.uButtAdditionalMission)
        {
            item.Activate(activate);

            if (activate)
            {
                this.regionControler.AskManagerToMission(item, this);
            }
        }
    }

    public Image GetImage()
    {
        return this.image;
    }

    public Region GetRegion()
    {
        return this.region;
    }



}
