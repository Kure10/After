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


    public void ChoiseMission()
    {
       
        int i = Random.Range(0, allMissions.Count);
        Mission mission = allMissions[i];


        ShowMissionPanel(mission);

    }

    private void ShowMissionPanel(Mission mission)
    {
        theMC.windowMission.MissionName = mission._name;
        theMC.windowMission.MissionType = mission.type;
        theMC.windowMission.MissionDistance = mission.distance;
        theMC.windowMission.MissionLevel = mission.levelOfDangerous;
        theMC.windowMission.MissionEnviroment = mission.missionEnviroment;
        theMC.windowMission.MissionTime = mission.missionTime;
        theMC.windowMission.Sprite = mission.image;

        Button startButton = theMC.windowMission.GetStartMissionButton;

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(delegate () { theMC.StartMission(mission); });

        theMC.windowMission.Init();

        currentMission = mission;

        theMC.windowMission.SetActivityMissionPanel = true;
    }





}
