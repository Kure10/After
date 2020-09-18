using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowMissionController : MonoBehaviour
{

    [SerializeField] uWindowMission uWindow;

    private MissionPanelState state;

    private Mission currentMission;

    public MissionPanelState State { set { this.state = value; } }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (this.state)
        {
            case MissionPanelState.normal:
                break;
            case MissionPanelState.inRepeatTime:
                if(currentMission != null)
                    uWindow.ButtonStartText = "Mission aktivni za: " + currentMission.RepeatableTime;
                break;
            case MissionPanelState.inProgress:
                if (currentMission != null)
                    uWindow.ButtonStartText = "Zrusit missi";
                break;
            case MissionPanelState.notComplete:
                break;

            default:
                break;
        }

    }

    public void SetUpWindow(Mission mission)
    {
        this.currentMission = mission;

        uWindow.MissionName = mission.Name;
        uWindow.MissionType = mission.ConvertMissionTypeStringData(mission.Type);
        uWindow.MissionDistance = mission.Distance;
        uWindow.MissionLevel = mission.LevelOfDangerous;
        uWindow.MissionTerrainList = mission.GetEmergingTerrains;
        uWindow.MissionTime = mission.MissionTime;
        uWindow.Sprite = mission.Image;
    }

    public Button GetWindowButton()
    {
        return uWindow.GetStartMissionButton;
    }

    public void Init()
    {
        uWindow.Init();
    }

    public void SetActivePanel(bool value)
    {
        uWindow.SetActivityMissionPanel = value;
    }

    public enum MissionPanelState { normal , inRepeatTime , inProgress , notComplete}

}
