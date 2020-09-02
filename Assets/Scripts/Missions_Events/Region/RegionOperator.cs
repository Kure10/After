using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionOperator : MonoBehaviour
{
    [Header("Prefabs")]
     private Region region;
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
        if(this.regionControler == null)
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

    public void CompleteMission(bool isRepeate , long missionID , bool missionCompleteConditionCounter = false)
    {
        if(missionCompleteConditionCounter)
            this.regionControler.onRegionCounterDecreased(this.region);


        if (isRepeate)
        {
            foreach (uButtonAdditionalMission item in this.uButtAdditionalMission)
            {
                if (item.CurrentMission.Id == missionID)
                    item.ChangeMissionOnClickEvent(this.regionControler.onButtonIsDeActivate);
            }
        }
        else
        {
            foreach (uButtonAdditionalMission item in this.uButtAdditionalMission)
            {
                if (item.CurrentMission.Id == missionID)
                    item.ChangeMissionOnClickEvent(this.regionControler.onButtonIsAlreadyCompleted);

            }
        }

        //  regionControler.RefreshRegions();
        
    }

    public void RefreshMissionButton(Mission mission)
    {
        foreach (uButtonAdditionalMission item in this.uButtAdditionalMission)
        {
            if (item.CurrentMission.Id == mission.Id)
                item.ChangeMissionOnClickEvent(this.regionControler.onButtonIsReActivate);

        }
    }

    public void InicializationRegion(RegionControler regionControler)
    {
        if (regionControler == null)
            this.regionControler = regionControler;

        this.image = GetComponent<Image>();
        image.sprite = this.region.Sprite;
       // image.color = new Color(255, 255, 255, 255);
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
        if (this.region.IsInDarkness)
        {
            Debug.Log("Region is in dargknesss  -> ToDo");
            return;
        }
        if (this.region.IsExplored)
        {
            Debug.Log("Region is Explore  -> ToDo");
            return;
        }
        if (regionControler.AskControllerIsMissionInProgress(region.MapArena))
        {
            Debug.Log("Mission is in progress");
            return;
        } 
        if (this.region.MissCompReq <= 0 || this.region.IsStartingRegion)
        {
            regionControler.StartExplore(this);
        }
        else
        {
            Debug.Log("Region: " + this.region.RegionName +
                      "  nelze prozkoumat, neni dokonceno spravne mnozstvi questu v okolnim regionu: " +
                      this.region.MissCompReq);
        }
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
