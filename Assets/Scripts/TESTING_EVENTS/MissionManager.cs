using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{


    [SerializeField]
    private MissionController theMC;

    /*tady mam vsechny misse*/
    public List<Mission> allMissions = new List<Mission>();

    public Mission currentMission;


    public void ChoiseMission(RegionOperator regionOperator)
    {
       
        int i = Random.Range(0, allMissions.Count);
        Mission mission = allMissions[i];


        ShowMissionPanel(mission, regionOperator);

    }

    private void ShowMissionPanel(Mission mission,RegionOperator regionOperator)
    {
        theMC.windowMission.MissionName = mission.Name;
        theMC.windowMission.MissionType = mission.Type;
        theMC.windowMission.MissionDistance = mission.Distance;
        theMC.windowMission.MissionLevel = mission.LevelOfDangerous;
        theMC.windowMission.MissionTerrainList = mission.GetEmergingTerrains;
        theMC.windowMission.MissionTime = mission.MissionTime;
        theMC.windowMission.Sprite = mission.Image;

        Button startButton = theMC.windowMission.GetStartMissionButton;

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(delegate () { theMC.StartMission(mission, regionOperator); });

        theMC.windowMission.Init();

        currentMission = mission;

        theMC.windowMission.SetActivityMissionPanel = true;
    }





}
