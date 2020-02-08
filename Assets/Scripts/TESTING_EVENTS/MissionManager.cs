using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    [SerializeField]
    uWindowMission windowMission;

    public List<Mission> missions = new List<Mission>();



    public void ChoiseMission()
    {
        int i = Random.Range(0, missions.Count);
        Mission mission = missions[i];


        ShowMissionPanel(mission);

    }

    private void ShowMissionPanel(Mission mission)
    {
        this.windowMission.MissionName = mission.missionName;
        this.windowMission.MissionType = mission.missionType;
        this.windowMission.MissionDistance = mission.missionDistance;
        this.windowMission.MissionLevel = mission.levelOfDangerous;
        this.windowMission.MissionEnviroment = mission.missionEnviroment;
        this.windowMission.MissionTime = mission.missionTime;
        this.windowMission.Sprite = mission.image;

        this.windowMission.Init();

        // napln specialisty..

        this.windowMission.SetActivityMissionPanel = true;
    }


}
