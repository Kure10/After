using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowMissionController : MonoBehaviour
{
    [SerializeField] private SpecialistControler theSC;

    [SerializeField] uWindowMission uWindowShowMission;

    [SerializeField] uWindowMissionSpecSelection uWindowSpecSelection;

    private MissionPanelState state;

    private Mission currentMission;

    private List<Specialists> specialistReadyToMission = new List<Specialists>();

    private List<GameObject> PlusButtons = new List<GameObject>();

    public MissionPanelState State { set { this.state = value; } }

    


    public void OnEnable()
    {
        switch (this.state)
        {
            case MissionPanelState.normal:
                uWindowShowMission.ButtonStartText = "Zacit missi";
                break;
            case MissionPanelState.inRepeatTime:
                if (currentMission != null)
                    CalcTimeForReactivationMission(currentMission.RepeatableTime);
                break;
            case MissionPanelState.inProgress:
                if (currentMission != null)
                    uWindowShowMission.ButtonStartText = "Zrusit missi";
                break;
            case MissionPanelState.Complete:
                if (currentMission != null)
                    uWindowShowMission.ButtonStartText = "Misse je splnena";
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

        uWindowShowMission.ButtonStartText = missionAvailableIn;
    }

    private void OpenSelectionPanel()
    {
        uWindowSpecSelection.gameObject.SetActive(true);
    }

    private void PrepairSelectionWindow(List<Specialists> specList)
    {
        var prefab = this.uWindowSpecSelection.SpecPrefab;
        var holder = this.uWindowSpecSelection.SpecHolder;

        foreach (Transform item in holder.transform)
        {
            Destroy(item);
        }

        foreach (Specialists item in specList)
        {
            var go = Instantiate(prefab, holder.transform);
            var uWindow = go.GetComponent<uWindowSpecialist>();
            uWindow.SetAll(item);

            var but = go.GetComponent<Button>();
            if (but != null)
            {
                but.onClick.RemoveAllListeners();
                but.onClick.AddListener( () => AddSpecialistToMission(item));
            }
        }
    }


    private void AddSpecialistToMission(Specialists spec)
    {

        specialistReadyToMission.Add(spec);

        var count = specialistReadyToMission.Count;

        this.uWindowShowMission.SetSpecMinMaxText(count, uWindowShowMission.GetMaxSpecOnMission);

        var plusButtonCount = this.PlusButtons.Count - 1;
        GameObject goToRemove = this.PlusButtons[plusButtonCount];
        this.PlusButtons.Remove(goToRemove);
        Destroy(goToRemove);

        var prefab = this.uWindowSpecSelection.SpecPrefab;
        var content = this.uWindowShowMission.SpecContent;

        var specGameObject = Instantiate(prefab,content.transform);
        var uWindow = specGameObject.GetComponent<uWindowSpecialist>();
        uWindow.SetAll(spec);

        uWindowSpecSelection.gameObject.SetActive(false);

    }


    public void SetUpWindow(Mission mission)
    {
        // for animation
        this.currentMission = mission;

        // setup SpecSelectionWindow
        List<Specialists> spec = this.theSC.PassSpecToMissionSelection();
        this.PrepairSelectionWindow(spec);

        // setup
        uWindowShowMission.MissionName = mission.Name;
        uWindowShowMission.MissionType = mission.ConvertMissionTypeStringData(mission.Type);
        uWindowShowMission.MissionDistance = mission.Distance;
        uWindowShowMission.MissionLevel = mission.LevelOfDangerous;
        uWindowShowMission.MissionTerrainList = mission.GetEmergingTerrains;
        uWindowShowMission.MissionTime = mission.MissionTime;
        uWindowShowMission.DesriptionText = mission.Description;
        uWindowShowMission.Sprite = mission.Image;
        uWindowShowMission.SetSpecMinMaxText(mission.SpecMin , mission.SpecMax);
    }

    public void CreateSpecAddButton(Mission mission)
    {
        var prefab = uWindowShowMission.SpecPrefab;
        var holder = uWindowShowMission.SpecContent;
        var prefabCount = mission.SpecMin + 1;

        foreach (Transform item in holder.transform)
        {
            Destroy(item);
        }

        for (int i = 0; i < prefabCount; i++)
        {
            var go = Instantiate(prefab, holder.transform);
            var button = go.GetComponent<Button>();
            this.PlusButtons.Add(go);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OpenSelectionPanel);
        }

    }

    public Button GetWindowButton()
    {
        return uWindowShowMission.GetStartMissionButton;
    }

    public void Init()
    {
        uWindowShowMission.Init();
    }

    public void SetActivePanel(bool value)
    {
        uWindowShowMission.SetActivityMissionPanel = value;
    }

    public enum MissionPanelState { normal , inRepeatTime , inProgress , Complete}

}
