using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{

    [SerializeField]
    uWindowMission windowMission;

    [SerializeField]
    private MissionController missionController;

    public List<Mission> missions = new List<Mission>();

    public Mission currentMission;


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

        Button startButton = this.windowMission.GetStartMissionButton;
        startButton.onClick.AddListener(delegate () { missionController.StartMission(mission); });



        this.windowMission.Init();

        currentMission = mission;

        this.windowMission.SetActivityMissionPanel = true;
    }





}
