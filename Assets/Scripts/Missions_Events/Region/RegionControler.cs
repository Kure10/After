using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RegionControler : MonoBehaviour
{

    public float fogValue = 0.25f;
    public Color black = new Color(0, 0, 0, 1f);
    public Color idleColor = new Color(1, 1, 1, 1);

    [SerializeField] private RegionOperator[] arrayOfRegionsOperator;
    private MissionManager missionManager;
    private MissionController missionController;

    [SerializeField] private MissionMapNotificationControler notificationControler;

    public UnityAction onButtonIsDeActivate;

  //  public UnityAction<uButtonAdditionalMission, RegionOperator> onButtonIsReActivate;

    public UnityAction onButtonIsAlreadyCompleted;

    public UnityAction<Region> onRegionCounterDecreased;

    public UnityAction<RegionOperator> onRegionCounterLimitWasNotReached;


    private void Awake()
    {
        missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
        missionController = FindObjectOfType<MissionController>();
    }

    public void Start()
    {
        onButtonIsDeActivate += AdditionMissionIsDisabled;
        onButtonIsAlreadyCompleted += AdditionMissionIsAlreadyCompleted;
        onRegionCounterDecreased += RegionCounterDecreased;
        onRegionCounterLimitWasNotReached += RegionCounterLimitNotReached;
    }

    public void ChangeRegionState(Region region, Image regionImage)
    {
        ChangeColoring(region, regionImage);
    }

    public void RefreshRegions()
    {
        foreach (RegionOperator item in arrayOfRegionsOperator)
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

    public void StartExplore(RegionOperator regionOperator)
    {
        missionManager.ChoiseMission(regionOperator, true);
    }

    public void AskManagerToMission(uButtonAdditionalMission button, RegionOperator regionOperator)
    {
        missionManager.ChoiseMissionForRegionButton(button, regionOperator);
    }

    public bool AskControllerIsMissionInProgress(string missionId)
    {
        return missionController.IsMissionInProgress(missionId);
    }

    public void SetRegions(List<Region> allRegions)
    {
        foreach (RegionOperator item in arrayOfRegionsOperator)
        {
            foreach (Region reg in allRegions)
            {
                if (item.RegionIdentifikator == reg.MapArena)
                {
                    item.Region = reg;
                    item.InicializationRegion(this);
                    break;
                }
            }
        }
    }

    #region Event Methods

    public void AdditionMissionIsDisabled()
    {
        Debug.Log("Disabled no action for now...");
    }

    public void AdditionMissionIsAlreadyCompleted()
    {
        Debug.Log("Mission Was completed...");
    }

    public void RegionCounterDecreased(Region reg)
    {
        Debug.Log("Mission counter deleted..");
        foreach (RegionOperator item in arrayOfRegionsOperator)
        {
            item.Region.MissCompReq -= 1;
        }
    }

    public void RegionCounterLimitNotReached(RegionOperator regionOperator)
    {
        Debug.Log("Mission counter deleted..");

        notificationControler.AddNotification(regionOperator);
    }

    #endregion
}
