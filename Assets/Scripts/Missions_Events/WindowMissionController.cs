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

    public void OnEnable()
    {
        switch (this.state)
        {
            case MissionPanelState.normal:
                uWindow.ButtonStartText = "Zacit missi";
                break;
            case MissionPanelState.inRepeatTime:
                if (currentMission != null)
                    CalcTimeForReactivationMission(currentMission.RepeatableTime);
                break;
            case MissionPanelState.inProgress:
                if (currentMission != null)
                    uWindow.ButtonStartText = "Zrusit missi";
                break;
            case MissionPanelState.Complete:
                if (currentMission != null)
                    uWindow.ButtonStartText = "Misse je splnena";
                break;

            default:
                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void CalcTimeForReactivationMission(float missionTime)
    {
        string missionAvailableIn = null;

        int seconds = (int)(missionTime % 60);
        int minutes = (int)(missionTime / 60) % 60;
        int hours = (int)(missionTime / 3600) % 24;
        int days = (int)(missionTime / 86400);


        string sec = seconds.ToString("D2");
        string min = minutes.ToString("D2");
        string hour = hours.ToString("D2");
        string dayS = days.ToString();


        missionAvailableIn = $"Misse dostupna za dnu:{dayS} hodin:{hour} min:{min} sec:{sec}";

        uWindow.ButtonStartText = missionAvailableIn;
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
        uWindow.DesriptionText = mission.Description;
        uWindow.Sprite = mission.Image;
    }

    public void CreateSpecListForMission (List<Specialists> specList)
    {
        var prefab = uWindow.SpecPrefab;
        var holder = uWindow.SpecContent;

        foreach (Transform item in holder.transform)
        {
            Destroy(item);
        }

        foreach (var item in specList)
        {
            var go = Instantiate(prefab, holder.transform);
            var uWindow = go.GetComponent<uWindowSpecialist>();
            uWindow.SetAll(item);
        }

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

    public enum MissionPanelState { normal , inRepeatTime , inProgress , Complete}

}
