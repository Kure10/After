using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionOperator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Region region;

    [SerializeField] private string exploreMissionID;

    [SerializeField] List<uButtonAdditionalMission> uButtAdditionalMission = new List<uButtonAdditionalMission>();

    private RegionControler regionControler;

    private Image image;

    #region Properities

    public string ExploreMissionID { get { return this.exploreMissionID; } }

    #endregion

    private void Awake()
    {
        this.regionControler = GameObject.FindObjectOfType<RegionControler>();
        image = GetComponent<Image>();
        image.sprite = this.region.GetSprite;
        InicializationRegion();
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

    public void CompleteMission(bool isRepeate , long missionID)
    {
        // is repeate budu potrebovat na to abych schoval tlacitko nebo ukazal

        foreach (Region item in region.neighborhoodRegions)
        {
            item.AmountToUnlockedNeighborhodRegions -= 1;
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
            // neni opakovatelne
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

        long exploreMissionId;
        bool result = long.TryParse(exploreMissionID,out exploreMissionId);
        if (regionControler.AskControllerIsMissionInProgress(exploreMissionId))
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
