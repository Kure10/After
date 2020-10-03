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

    private List<GameObject> SiblingsObject = new List<GameObject>();

    public MissionPanelState State { set { this.state = value; } }


    private void Awake()
    {
        var button = uWindowShowMission.GetCloseMissionButton;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(DisableShowMissionPanel);
    }

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
        List<Specialists> list = this.theSC.PassSpecToMissionSelection();
        this.PrepairSelectionWindow(list);
        uWindowSpecSelection.gameObject.SetActive(true);
    }

    private void PrepairSelectionWindow(List<Specialists> specList)
    {
        var prefab = this.uWindowSpecSelection.SpecPrefab;
        var holder = this.uWindowSpecSelection.SpecHolder;

        foreach (Transform item in holder.transform)
        {
            Destroy(item.gameObject);
        }

        foreach (Specialists item in specList)
        {
            var go = Instantiate(prefab, holder.transform);
            var uWindow = go.GetComponent<uWindowSpecialist>();
            uWindow.SetAll(item);

            var but = go.GetComponent<Button>();

            if (but == null)
            {
                Debug.LogError("Error Nejaky buttonSpecialist prefab nemel button component");
                continue;
            }

            but.onClick.RemoveAllListeners();

            if (item.isSelectedOnMission)
            {
                uWindow.SetSuperimposePanel(true,"Specialista je už vybrat.");
               // but.onClick.AddListener(() => AddSpecialistToMission(item));
                // Some Action Todo
            }
            else if (item.isOnMission)
            {
                uWindow.SetSuperimposePanel(true, "Specialista je na missi.");
                // Some Action Todo
            }
            else
            {
                uWindow.SetSuperimposePanel(false);
                but.onClick.AddListener(() => AddSpecialistToMission(item));
            }

        }
    }


    private void AddSpecialistToMission(Specialists spec)
    {
        spec.isSelectedOnMission = true;

        specialistReadyToMission.Add(spec);

        var specCount = specialistReadyToMission.Count;

        this.uWindowShowMission.SetSpecMinMaxText(specCount, uWindowShowMission.GetMaxSpecOnMission);

        var siblingsCount = this.SiblingsObject.Count;

        if(siblingsCount > 0)
        {
            for (int i = siblingsCount -1  ; i >= 0; i--)
            {
                var ob = this.SiblingsObject[i];
                uWindowSpecialist uWindowspec = ob.GetComponent<uWindowSpecialist>();

                if (uWindowspec == null)
                {
                    this.SiblingsObject.Remove(ob);
                    Destroy(ob.gameObject);
                    break;
                }
            }
        }

        var prefab = this.uWindowSpecSelection.SpecPrefab;
        var content = this.uWindowShowMission.SpecContent;

        var specGameObject = Instantiate(prefab,content.transform);
        specGameObject.transform.SetAsFirstSibling();
        this.SiblingsObject.Add(specGameObject);

        var uWindow = specGameObject.GetComponent<uWindowSpecialist>();
        uWindow.SetAll(spec);
        uWindow.SetSuperimposePanel(false);

        uWindowSpecSelection.gameObject.SetActive(false);

        siblingsCount = this.SiblingsObject.Count;

        Debug.Log("specCount: " + specCount + " siblingsCount: " + siblingsCount +  "  currentMission.SpecMax-> " + currentMission.SpecMax);

        if (siblingsCount < currentMission.SpecMax)
        {
            Debug.Log("pridavam kurva");
            var pref = this.uWindowShowMission.PlusPrefab;
            var go = Instantiate(pref, content.transform);
            go.transform.SetAsLastSibling();
            var button = go.GetComponent<Button>();
            this.SiblingsObject.Add(go);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OpenSelectionPanel);
        }


        // deaktivovat button specialisty co byl vybrat..
        // zmenit barvu.


        // setup SpecSelectionWindow
        //List<Specialists> list = this.theSC.PassSpecToMissionSelection();
        //this.PrepairSelectionWindow(list);
    }

    private void RemoveSpecialistToMission(GameObject gameObjectToRemove, Specialists spec)
    {
        spec.isSelectedOnMission = false;
        specialistReadyToMission.Remove(spec);

        var specCount = specialistReadyToMission.Count;

        this.uWindowShowMission.SetSpecMinMaxText(specCount, uWindowShowMission.GetMaxSpecOnMission);


        var siblingsCount = this.SiblingsObject.Count;

        if (siblingsCount > 0)
        {
            for (int i = siblingsCount - 1; i >= 0; i--)
            {
                var ob = this.SiblingsObject[i];
                uWindowSpecialist uWindowspec = ob.GetComponent<uWindowSpecialist>();

                //if (uWindowspec != null && )
                //{
                //    this.SiblingsObject.Remove(ob);
                //    Destroy(ob.gameObject);
                //    break;
                //}
            }
        }


        var plusButtonCount = this.SiblingsObject.Count;
        if (plusButtonCount > 0)
        {
            GameObject goToRemove = this.SiblingsObject[plusButtonCount - 1];
            this.SiblingsObject.Remove(goToRemove);
            Destroy(goToRemove);
        }

        var prefab = this.uWindowSpecSelection.SpecPrefab;
        var content = this.uWindowShowMission.SpecContent;

        var specGameObject = Instantiate(prefab, content.transform);
        specGameObject.transform.SetAsFirstSibling();
        var uWindow = specGameObject.GetComponent<uWindowSpecialist>();
        uWindow.SetAll(spec);

        // nastavi u uWindow zakryvaci panel na vypnut..
        //uWindow.SetSuperimposePanel(false);

        // vypne okno
        //uWindowSpecSelection.gameObject.SetActive(false);

        plusButtonCount = this.SiblingsObject.Count;
        int amountOfFullSlots = plusButtonCount + specCount;

        Debug.Log("specCount: " + specCount + " plusButtonCount: " + plusButtonCount + " amountOfFullSlots: " + amountOfFullSlots + "  currentMission.SpecMax-> " + currentMission.SpecMax);

        if (amountOfFullSlots < currentMission.SpecMax)
        {
            var pref = this.uWindowShowMission.PlusPrefab;
            var go = Instantiate(pref, content.transform);
            go.transform.SetAsLastSibling();
            var button = go.GetComponent<Button>();
            this.SiblingsObject.Add(go);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OpenSelectionPanel);
        }
    }


    public void SetUpWindow(Mission mission)
    {
        // for text StartButton
        this.currentMission = mission;

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
        var prefab = uWindowShowMission.PlusPrefab;
        var holder = uWindowShowMission.SpecContent;
        var prefabCount = mission.SpecMin + 1;

        this.SiblingsObject.Clear();

        foreach (Transform item in holder.transform)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < prefabCount; i++)
        {
            var go = Instantiate(prefab, holder.transform);
            var button = go.GetComponent<Button>();
            this.SiblingsObject.Add(go);
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

    private void DisableShowMissionPanel()
    {
        foreach (Specialists spec in this.specialistReadyToMission)
        {
            if (spec.isSelectedOnMission)
                spec.isSelectedOnMission = false;
        }

        this.uWindowShowMission.gameObject.SetActive(false);

        this.SiblingsObject.Clear();
        this.specialistReadyToMission.Clear();
    }


    public enum MissionPanelState { normal , inRepeatTime , inProgress , Complete}

}
