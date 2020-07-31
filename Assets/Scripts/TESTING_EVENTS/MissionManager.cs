using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{


    [SerializeField]
    private MissionController theMC;


    public List<Mission> exploreMissions = new List<Mission>();
    public List<Mission> othersMissions = new List<Mission>();

    public Mission currentMission;

    public void ChoiseMission(RegionOperator regionOperator, bool isExploreMission = false)
    {
        int i = 0;
        Mission mission = new Mission();
        // vyber explore missi nebo vsechny ostatni..
        if (isExploreMission)
        {
            i = Random.Range(0, exploreMissions.Count);
            mission = exploreMissions[i];
        }
        else
        {
            i = Random.Range(0, othersMissions.Count);
            mission = othersMissions[i];
        }

        ShowMissionPanel(mission, regionOperator);

    }

    private void ShowMissionPanel(Mission mission,RegionOperator regionOperator)
    {
        theMC.windowMission.MissionName = mission.Name;
        theMC.windowMission.MissionType = mission.ConvertMissionTypeStringData(mission.Type);
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
